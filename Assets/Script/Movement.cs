using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {
	
	public float acceleration = 1;
	public float maxMovementSpeed = 6;
	public float maxMovSpeedChange = 1;
	public Vector2 maxVelocity = new Vector2(10,10);
	public float jumpForce = 5;
	public Vector2 gravity = new Vector2(0f, 9.82f);
	public bool grounded;
	public float extendedRayDistance = 0.1f;
	public bool playerControl = true;
	
	//network stuff
	private RobNet robNet;
	public NetworkViewID viewID;
	public bool isLocal = true;
	private float lastSyncTime = 0f;
	public Vector3 startSyncPosition = Vector3.zero;
	public Vector3 endSyncPosition = Vector3.zero;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	
	//public int nrSavedStates = 2;
	//public List<State> prevSyncs = new List<State>();
	private State currentState = new State();
	private State lastState = new State();
	//private Vector3 lastVelocity = Vector3.zero;
	
	private Vector3 syncPosition = Vector3.zero;
	private Vector3 syncVelocity = Vector3.zero;
	

	[System.Serializable]
	public class State{
		
		public float timestamp;
		public Vector3 pos;
		public Vector3 velocity;
		
		public void copy(State s){
			this.timestamp = s.timestamp;
			this.pos = s.pos;
			this.velocity = s.velocity;
		}

	};


	// Use this for initialization
	void Start () {
		rigidbody.useGravity = false;
		isLocal = networkView.isMine;
		rigidbody.isKinematic = !isLocal;
		lastSyncTime = Time.time;

		//State state = new State();
		//state.pos = rigidbody.position;
		//state.velocity = rigidbody.velocity;
		//state.timestamp = Time.time;
		//prevSyncs.Add(state);
		//robNet = GameObject.Find("Mastermind").GetComponent<RobNet>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(isLocal){
			
			grounded = isGrounded();
			Vector3 targetVelocity = Vector3.zero;
			bool control = isPlayerInControl();
			if(control){
				Vector3 velocity = rigidbody.velocity;
				/* //movement med knappar, kan ha kvar den så länge
				if(Input.GetKey(KeyCode.D) || Input.GetButton("Right")){
                	targetVelocity += velocity + (Vector3.right * acceleration);
				}
				if(Input.GetKey(KeyCode.A) || Input.GetButton("Left")){
                	targetVelocity += velocity + (Vector3.left * acceleration);
				}*/
				float horizontal = Input.GetAxis("Horizontal");
				targetVelocity += velocity + (new Vector3(horizontal,0,0) * acceleration);
				
				
				targetVelocity.x = Mathf.Clamp(targetVelocity.x,-maxMovementSpeed,maxMovementSpeed);
				
				if(Input.GetButton("Jump") && grounded){
                	
					rigidbody.velocity = new Vector3(velocity.x,jumpForce,0);
					grounded = false;
				}

			}
			
			Vector3 velocityChange = targetVelocity - rigidbody.velocity;
			velocityChange.x = Mathf.Clamp(velocityChange.x,-maxMovSpeedChange,maxMovSpeedChange);
			velocityChange.y = 0;//Mathf.Clamp(velocityChange.y,-maxVelocityChange.y,maxVelocityChange.y);
			velocityChange.z = 0; //bara för att vara säker
			
			rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
			//if(!grounded){
				rigidbody.AddForce(new Vector3(gravity.x*rigidbody.mass,-gravity.y*rigidbody.mass,0));
			//}
			
		}else{
			nonLocalUpdate();
		}
		
		
	}
	
	bool isGrounded(){
		float rayDist = collider.bounds.extents.y + extendedRayDistance;
		RaycastHit hit;
		Vector3 pos = rigidbody.position;
		//Behöver specifiera layermasks så det funkar korrect
		/* //håll kvar den här ifall vi behöver byta cast om raycast inte duger
		float radius = collider.bounds.size.x/3f*2f;
		if(Physics.SphereCast(pos,radius,-Vector3.up,out hit,rayDist)){ 
			return true;
		}*/
		Debug.DrawLine(collider.bounds.center, collider.bounds.center + rayDist * -Vector3.up);

		if(Physics.Raycast(collider.bounds.center,-Vector3.up,out hit,rayDist)){
			return true;
		}
		return false;
	}
	
	bool isPlayerInControl(){
		//gör kollar om tex spelaren är stunnad osv
		return playerControl; //&& !slide;
	}
	
	void nonLocalUpdate(){
		syncTime = Time.time - currentState.timestamp;

		Vector3 newPosition = Vector3.Lerp(startSyncPosition,endSyncPosition, syncTime/syncDelay);
		rigidbody.position = newPosition;
		if(Input.GetKeyUp(KeyCode.Z)){
			Debug.Log ("ST " + syncTime + "SD " + syncDelay + "ST/SD " + syncTime/syncDelay);
		}
		

	}
	
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		if(stream.isWriting){
			
			syncPosition = rigidbody.position;
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
		}else{
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);

			lastState.copy (currentState); //save the previous state

			currentState.pos = syncPosition;
			currentState.velocity = syncVelocity;
			currentState.timestamp = Time.time;

			//if(prevSyncs.Count > nrSavedStates){
			//	prevSyncs.RemoveAt(0);
			//}
			
			syncDelay = Time.time - lastState.timestamp;
			syncTime = 0f;   				//reset the value, used for interpolation
			
			syncDelay = Mathf.Clamp(syncDelay,0.001f,30f);
			
			Vector3 velocityDelta = lastState.velocity - syncVelocity;
			Vector3 acceleration = velocityDelta/syncDelay;

			endSyncPosition = syncPosition + syncVelocity * syncDelay + 0.5f*acceleration*Mathf.Pow(syncDelay,2f); // Pt = P0+V0*T + 1/2*A*T^2
			startSyncPosition = rigidbody.position;

			if(syncVelocity.magnitude < 0.05f){
				endSyncPosition = startSyncPosition;
			}
			
		}
		
	}
	
	/*
	void SlideCheck(){
		raycastCounter++;
		if(raycastCounter % 10 == 0){
			slide = false;
			RaycastHit hit;
			float rayDist = (transform.localScale.y/2f)+0.1f; //ändras beroende på hur modellerna blir sen
			if(Physics.Raycast(transform.position,-Vector3.up,out hit,rayDist)){
				if(Vector3.Angle(hit.normal,Vector3.up) > slideLimit){
					slide = true;
					playerControl = false;
				}
			}
			if(slide){
				Vector3 hitNormal = hit.normal;
				velocityChange += new Vector3(hitNormal.x,-hitNormal.y,hitNormal.z)*slideSpeed;
			}else{
				playerControl = true; //får ändra detta sen
			}
		}
	}
	*/
}


