using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class Movement : MonoBehaviour {
	
	public float movementAcceleration = 1;
	public float extendedRayDistance = 0.35f;
	public float gravity =  9.82f;
	public float jumpForce = 12;
	public float maxMovementSpeed = 8;
	public float maxMovementSpeedChange = 1.5f;
	public float rotSpeed = 1;
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

	//messages are only sent by the network owner of the player, all message receiver methods assume this
	void FixedUpdate () {
		if(isLocal == false) {
			return;
		}

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		var wasGrounded = grounded;
		grounded = isGrounded();
		var becameGrounded = wasGrounded == false && grounded;
		var becameUnGrounded = wasGrounded && grounded == false;
		var beganUseItemLeft = Input.GetButton("UseItemLeft");
		var beganUseItemRight = Input.GetButton("UseItemRight");
		var beganJumping = grounded && Input.GetButton("Jump");

		var fi = calculateForce(horizontal, vertical);
		if(playerControl) {
			rigidbody.rotation = calculateRotation(horizontal, vertical);
			rigidbody.AddForce(fi.velocityChange, ForceMode.VelocityChange);

			if(becameGrounded) {
				SendMessage("airEnd");
			} else if(beganJumping) { 
				SendMessage("jumpBegin");
			} else if(becameUnGrounded) {
				SendMessage("airBegin");
			} else if(beganUseItemLeft) {
				SendMessage("useItemLeftBegin");
			} else if(beganUseItemRight) {
				SendMessage("useItemRightBegin");
			}
		}

		if(inJumpAnimation == false) { //jump animation has no gravity
			rigidbody.AddForce(fi.gravity);
		}
	}

	struct ForceInfo {
		public ForceInfo(Vector3 vc, Vector3 g) {
			velocityChange = vc;
			gravity = g;
		}
		public Vector3 velocityChange;
		public Vector3 gravity;
	}

	ForceInfo calculateForce(float horizontal, float vertical) {
		var targetVelocity = Vector3.zero;
		targetVelocity += rigidbody.velocity + (new Vector3(horizontal,0,vertical) * movementAcceleration);
		targetVelocity.x = Mathf.Clamp(targetVelocity.x,-maxMovementSpeed,maxMovementSpeed);
		targetVelocity.z = Mathf.Clamp(targetVelocity.z,-maxMovementSpeed,maxMovementSpeed);

		Vector3 velocityChange = targetVelocity - rigidbody.velocity;
		velocityChange.x = Mathf.Clamp(velocityChange.x,-maxMovementSpeedChange,maxMovementSpeedChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z,-maxMovementSpeedChange,maxMovementSpeedChange);
		velocityChange.y = 0; //since we should not move verticaly
		return new ForceInfo(velocityChange, new Vector3(0,-gravity*rigidbody.mass,0));
	}

	Quaternion calculateRotation(float horizontal, float vertical) {
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
		return Quaternion.Euler(0,currentDirection,0);
	}

	bool isGrounded(){
		float rayDist = collider.bounds.extents.y + extendedRayDistance;
		RaycastHit hit;
		return Physics.Raycast(collider.bounds.center, -Vector3.up, out hit, rayDist,layerMask.value);
	}

	IEnumerator useItemTurnAround() {
		var hit = PhysicsUtility.mouseHit();
		var xzdir = hit.point.XZ() - transform.position.XZ();
		var dir = new Vector3(xzdir.x, 0, xzdir.y).normalized;

		var beginRot = rigidbody.rotation;
		var endRot = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.z), 0);

		var deltaEuler = Quaternion.Angle(beginRot, endRot);
		var endTime = Time.time + deltaEuler / rotSpeed; //should really be length of animation
		var beginTime = Time.time;
		var timeDiff = endTime - beginTime;
		while(true) {
			transform.rotation = Quaternion.Lerp(beginRot, endRot, Mathf.Clamp((Time.time - beginTime) / timeDiff, 0, 1));
			if(Time.time >= endTime) {
				yield break;
			} else {
				yield return new WaitForFixedUpdate();
			}
		}
	}

	//Callbacks
	
	void jumpBegin() {
		playerControl = false;
		inJumpAnimation = true;
	}
	
	void jumpEnd() {
		playerControl = true;
		inJumpAnimation = false;
		grounded = true; //probably safe to assume
	}

	void useItemLeftBegin() {
		playerControl = false;
		StartCoroutine(useItemTurnAround());
	}
	
	void useItemLeftEnd() {
		playerControl = true;
	}

	void useItemRightBegin() {
		playerControl = false;
		StartCoroutine(useItemTurnAround());
	}
	
	void useItemRightEnd() {
		playerControl = true;
	}
}


