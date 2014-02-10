using UnityEngine;
using System.Collections;

public class RigidbodySync : MonoBehaviour {

	private NetworkView netview;
	public bool isLocal = false;
	private Vector3 startSyncPosition = Vector3.zero; //position entity is at at the start of the sync
	private Vector3 endSyncPosition = Vector3.zero; //predicted position at next sync
	private float syncDelay = 0f;
	private float syncTime = 0f;
	
	private State currentState = new State();
	private State lastState = new State();
	
	private Vector3 syncPosition = Vector3.zero;
	private Vector3 syncVelocity = Vector3.zero;
	
	//rotation
	private Quaternion syncRotation;
	private Quaternion fromSyncRotation;

	[System.Serializable]
	public class State{
		
		public float timestamp;
		public Vector3 pos;
		public Vector3 velocity;
		public Quaternion rotation; 
		
		public void copy(State s){
			this.timestamp = s.timestamp;
			this.pos = s.pos;
			this.velocity = s.velocity;
			this.rotation = s.rotation;
		}
	};


	// Use this for initialization
	void Start () {
		netview = GetComponent<NetworkView>();
		isLocal = netview.isMine;
		syncRotation = transform.rotation;
		fromSyncRotation = transform.rotation;
		
		//rigidbody.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocal){
			nonLocalUpdate();
		}
	}

	void nonLocalUpdate(){
		syncTime = Time.time - currentState.timestamp;
		
		Vector3 newPosition = Vector3.Lerp(startSyncPosition,endSyncPosition, syncTime/syncDelay);
		
		rigidbody.position = newPosition;
		
		Quaternion newRotation = Quaternion.Lerp(fromSyncRotation, syncRotation, syncTime/syncDelay);
		rigidbody.rotation = newRotation;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		
		if(stream.isWriting){
			
			syncPosition = rigidbody.position;
			syncVelocity = rigidbody.velocity;
			syncRotation = rigidbody.rotation;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
		}else{
			//current and synced rotation is enough to predict rotation animation
			fromSyncRotation = rigidbody.rotation;
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
			
			lastState.copy (currentState); //save the previous state
			
			currentState.pos = syncPosition;
			currentState.velocity = syncVelocity;
			currentState.timestamp = Time.time;
			currentState.rotation = syncRotation;
			
			SendMessage("newVelocity", syncVelocity);
			
			syncDelay = Time.time - lastState.timestamp;
			syncTime = 0f;   				//reset the value, used for interpolation
			
			syncDelay = Mathf.Clamp(syncDelay,0.001f,30f); // makes sure we dont divide by 0 
			
			Vector3 velocityDelta = lastState.velocity - syncVelocity;
			Vector3 accelerationVector = velocityDelta/syncDelay;
			
			endSyncPosition = syncPosition + syncVelocity * syncDelay + 0.5f*accelerationVector*Mathf.Pow(syncDelay,2f); // Pt = P0+V0*T + 1/2*A*T^2
			startSyncPosition = rigidbody.position;
			
			/*if(syncVelocity.magnitude < 0.05f){		//this does for some reason cause problems when there are more than 2 players
				endSyncPosition = startSyncPosition;
			}*/
			
			
		}
		
	}
}
