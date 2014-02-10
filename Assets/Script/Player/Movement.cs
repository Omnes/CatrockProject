using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {
	
	public float acceleration = 1;
	public float maxMovementSpeed = 8;
	public float maxMovSpeedChange = 1.5f;
	public float jumpForce = 12;
	public float gravity =  9.82f;
	public bool grounded = true;
	public float extendedRayDistance = 0.35f;
	public bool playerControl = true;
	public float direction = 0f;

	public float turningSpeed = 5f;

	public bool isLocal = true;
	private bool movementEnabled = true;

	void itemAnimationBegin() {
	  movementEnabled = true;
	}

	void itemAnimationEnd() {
	  movementEnabled = true;
	}

	// Use this for initialization
	void Start () {
		rigidbody.useGravity = false;
		isLocal = networkView.isMine;
		rigidbody.isKinematic = !isLocal;
		
		movementEnabled = true;

	}

	void FixedUpdate () {
		if(movementEnabled == false) {
			return;
		}
		
		if(isLocal){
			var wasGrounded = grounded;
			grounded = isGrounded();

			if(wasGrounded == false && grounded) {
				SendMessage("airEnd");
			}

			bool control = isPlayerInControl();
			Vector3 targetVelocity = Vector3.zero;
			if(control){
				//get the input
				float horizontal = Input.GetAxis("Horizontal");
				float vertical = Input.GetAxis("Vertical");

				//rotation stuff
				float currentDirection = transform.eulerAngles.y;
				if(currentDirection > 179){
					currentDirection -= 360;
				}

				if(!fEqual(horizontal,0) || !fEqual(vertical,0)){ // if we have a new inputvector do rotation stuff
					float targetDirection = Mathf.Atan2(horizontal,vertical) * Mathf.Rad2Deg;
					float deltaDir = targetDirection - currentDirection;
					if(deltaDir > 180){
						deltaDir -= 360;
					}
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
					SendMessage ("airBegin");
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

		if(Physics.Raycast(collider.bounds.center,-Vector3.up,out hit,rayDist)){
			return true;
		}
		return false;
	}
	
	bool isPlayerInControl(){
		//gör kollar om tex spelaren är stunnad osv
		return playerControl; //&& !slide;
	}

	void castSpell(Vector3 direction) {
		
	}

}


