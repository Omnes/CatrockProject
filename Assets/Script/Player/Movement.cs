using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour {
	
	public float movementAcceleration = 1;
	public float extendedRayDistance = 0.35f;
	public float gravity =  9.82f;
	public float jumpForce = 12;
	public float maxMovementSpeed = 8;
	public float maxMovementSpeedChange = 1.5f;
	public float turningSpeed = 5f;

	//should not be changed during runtime, but for debugging purposes, could be observed.
	public bool grounded = true;
	public bool isLocal;
	public bool inJumpAnimation = false;
	public bool playerControl = true;
	public LayerMask layerMask;


	// Use this for initialization
	void Start () {
		rigidbody.useGravity = false;
		isLocal = networkView.isMine;
		rigidbody.isKinematic = !isLocal;
	}

	void FixedUpdate () {
		if(isLocal){
			var wasGrounded = grounded;
			grounded = isGrounded();
			var becameGrounded = wasGrounded == false && grounded;
			var becameUnGrounded = wasGrounded && grounded == false;

			Vector3 targetVelocity = Vector3.zero;

			if(playerControl) {
				//input
				float horizontal = Input.GetAxis("Horizontal");
				float vertical = Input.GetAxis("Vertical");

				//rotation
				float currentDirection = FloatUtility.normalizeEulerAngles(transform.eulerAngles.y);
				if(FloatUtility.fEqual(horizontal,0) == false || FloatUtility.fEqual(vertical,0) == false) { // if we have a new inputvector do rotation stuff
					float targetDirection = Mathf.Atan2(horizontal,vertical) * Mathf.Rad2Deg;
					float deltaDir = FloatUtility.normalizeEulerAngles(targetDirection - currentDirection);
					if (Mathf.Abs (deltaDir) > turningSpeed){
						currentDirection += turningSpeed*FloatUtility.getSign(deltaDir);
					} else{
						currentDirection = targetDirection;
					}
				}
				transform.rotation = Quaternion.Euler(0,currentDirection,0);

				//movement
				Vector3 velocity = rigidbody.velocity;
				targetVelocity += velocity + (new Vector3(horizontal,0,vertical) * movementAcceleration);
				targetVelocity.x = Mathf.Clamp(targetVelocity.x,-maxMovementSpeed,maxMovementSpeed);
				targetVelocity.z = Mathf.Clamp(targetVelocity.z,-maxMovementSpeed,maxMovementSpeed);

				//events
				if(becameGrounded) {
					OurDebug.Log("airEnd");
					SendMessage("airEnd");
				} else if(grounded && Input.GetButton("Jump")) { 
					SendMessage("jumpBegin");
				} else if(becameUnGrounded) {
					SendMessage("airBegin");
				}
			}

			if(inJumpAnimation == false) {
				Vector3 velocityChange = targetVelocity - rigidbody.velocity;
				velocityChange.x = Mathf.Clamp(velocityChange.x,-maxMovementSpeedChange,maxMovementSpeedChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z,-maxMovementSpeedChange,maxMovementSpeedChange);
				velocityChange.y = 0; //since we should not move verticaly
				rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
				//if(grounded == false) {
					rigidbody.AddForce(new Vector3(0,-gravity*rigidbody.mass,0));
				//}
			}
		}
	}

	bool isGrounded(){
		float rayDist = collider.bounds.extents.y + extendedRayDistance;
		RaycastHit hit;
		return Physics.Raycast(collider.bounds.center, -Vector3.up, out hit, rayDist,layerMask.value);
	}

	//Callbacks

	void itemAnimationBegin() {
		playerControl = false;
	}
	
	void itemAnimationEnd() {
		playerControl = true;
	}
	
	void jumpBegin() {
		playerControl = false;
		inJumpAnimation = true;
	}
	
	void jumpEnd() {
		playerControl = true;
		inJumpAnimation = false;
		grounded = true; //probably safe to assume
	}
}


