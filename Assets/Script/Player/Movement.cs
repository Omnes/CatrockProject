using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {
	
	public float acceleration = 1;
	public float maxMovementSpeed = 6;
	public float maxMovSpeedChange = 1;
	public float jumpForce = 5;
	public float gravity =  9.82f;
	public bool grounded;
	public float extendedRayDistance = 0.1f;
	public bool playerControl = true;
	public float direction = 0f;

	public float turningSpeed = 5f;
	
	//network stuff
	public bool isLocal = true;
	public Vector3 startSyncPosition = Vector3.zero; //position entity is at at the start of the sync
	public Vector3 endSyncPosition = Vector3.zero; //predicted position at next sync
	public float syncDelay = 0f;
	private float syncTime = 0f;
	
	//public int nrSavedStates = 2;
	//public List<State> prevSyncs = new List<State>();
	private State currentState = new State();
	private State lastState = new State();
	//private Vector3 lastVelocity = Vector3.zero;
	
	private Vector3 syncPosition = Vector3.zero;
	private Vector3 syncVelocity = Vector3.zero;
	
	//rotation
	public Quaternion syncRotation;
	public Quaternion fromSyncRotation;

	[System.Serializable]
	public class State{
		
		public float timestamp;
		public Vector3 pos;
		public Vector3 velocity;
		public Quaternion rotation; //is this needed? Can't see State used anywhere really. / it is see OnSerializeNetworkView
		
		public void copy(State s){
			this.timestamp = s.timestamp;
			this.pos = s.pos;
			this.velocity = s.velocity;
			this.rotation = s.rotation;
		}

	};


	// Use this for initialization
	void Start () {
		//rotation
		syncRotation = transform.rotation;
		fromSyncRotation = transform.rotation;

		rigidbody.useGravity = false;
		isLocal = networkView.isMine;
		rigidbody.isKinematic = !isLocal;

		//State state = new State();
		//state.pos = rigidbody.position;
		//state.velocity = rigidbody.velocity;
		//state.timestamp = Time.time;
		//prevSyncs.Add(state);
	}

	void FixedUpdate () {
		
		if(isLocal){
			grounded = isGrounded();
			bool control = isPlayerInControl();
			Vector3 targetVelocity = Vector3.zero;
			if(control){
				float currentDirection = transform.eulerAngles.y;
				if(currentDirection > 179){
					currentDirection -= 360;
				}
				float horizontal = Input.GetAxis("Horizontal");
				float vertical = Input.GetAxis("Vertical");

				if(!fEqual(horizontal,0) || !fEqual(vertical,0)){ // if we have a new inputvector do rotation stuff
					float targetDirection = Mathf.Atan2(horizontal,vertical) * Mathf.Rad2Deg;
					float deltaDir = targetDirection - currentDirection;
					if(deltaDir > 180){
						deltaDir -= 360;
					}
					Debug.Log ("delta " + deltaDir);
					if (Mathf.Abs (deltaDir) > turningSpeed){
						currentDirection += turningSpeed*getSign(deltaDir);
					} else{
						currentDirection = targetDirection;
					}
				}

				//now to the movement
				Vector3 velocity = rigidbody.velocity;

				targetVelocity += velocity + (new Vector3(horizontal,0,vertical) * acceleration);
				
				targetVelocity.x = Mathf.Clamp(targetVelocity.x,-maxMovementSpeed,maxMovementSpeed);
				targetVelocity.z = Mathf.Clamp(targetVelocity.z,-maxMovementSpeed,maxMovementSpeed);
				
				if(Input.GetButton("Jump") && grounded){
					
					rigidbody.velocity = new Vector3(velocity.x,jumpForce,velocity.z);
					grounded = false;
				}
				transform.rotation = Quaternion.Euler(0,currentDirection,0);
				
			}
			
			Vector3 velocityChange = targetVelocity - rigidbody.velocity;
			velocityChange.x = Mathf.Clamp(velocityChange.x,-maxMovSpeedChange,maxMovSpeedChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z,-maxMovSpeedChange,maxMovSpeedChange);
			velocityChange.y = 0; //since we should not move verticaly
			

			rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
			//if(!grounded){ // might be a good idea to add this back when grounded is working correctly
			rigidbody.AddForce(new Vector3(0,-gravity*rigidbody.mass,0));
			//}
		}else{
			nonLocalUpdate();
		}
		
		
	}
	//use to compare floats
	bool fEqual(float f1, float f2){
		float pres = 0.01f;
		if(f1-pres < f2 && f1 + pres > f2) 
			return true;
		return false;
	}

	int getSign(float n){
		if(n-0.01 < 0 && n + 0.01 > 0){ 
			return 0; 
		}
		if(n > 0){
			return 1;
		}
		if(n < 0){
			return -1;
		}
		return 0;
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

		//Debug.DrawLine(collider.bounds.center, collider.bounds.center + rayDist * -Vector3.up); //debug

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

			//if(prevSyncs.Count > nrSavedStates){
			//	prevSyncs.RemoveAt(0);
			//}
			
			syncDelay = Time.time - lastState.timestamp;
			syncTime = 0f;   				//reset the value, used for interpolation
			
			syncDelay = Mathf.Clamp(syncDelay,0.001f,30f); // makes sure we dont divide by 0 
			
			Vector3 velocityDelta = lastState.velocity - syncVelocity;
			Vector3 accelerationVector = velocityDelta/syncDelay;

			endSyncPosition = syncPosition + syncVelocity * syncDelay + 0.5f*accelerationVector*Mathf.Pow(syncDelay,2f); // Pt = P0+V0*T + 1/2*A*T^2
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


