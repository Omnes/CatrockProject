using UnityEngine;
using System.Collections;


public class AnimatePlayer : MonoBehaviour {
	private Animator animator;

	private float prevVelocity;
	private Quaternion rightFacingRot;
	private Quaternion leftFacingRot;
	private Quaternion leftRotStep;
	
	
	public enum AirRotateBehavior {
		Rotate,
		RotateIfArrowKeyDown,
		RotateAsIfNoInput,
		DontRotate
	};
	
	public float rotSpeed = 5f;
	public AirRotateBehavior airRotate;
	public float deltaRotSyncLimit = 0;

	private float normRot = 0f;
	private float lastDir = 0f;


	private Movement movement;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		leftFacingRot = new Quaternion();
		leftFacingRot.eulerAngles = Vector3.up * 269; //rotate 270 -> 180 -> 90, not 270 -> 0 -> 90
		rightFacingRot = new Quaternion();
		rightFacingRot.eulerAngles = Vector3.up * 91;
		leftRotStep = new Quaternion();
		leftRotStep.eulerAngles = Vector3.up * -rotSpeed;
		prevVelocity = 0f;

		movement = GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
		if(movement.isLocal) {
			setAnimatorSpeed(rigidbody.velocity.x);

			//dir is direction to face
			var dir = 	Input.GetAxis("Horizontal") < 0 ? -1 :
					Input.GetAxis("Horizontal") > 0 ? 1 : 0;

			if(movement.grounded == false) {
				if(airRotate == AirRotateBehavior.DontRotate || (dir == 0 && airRotate == AirRotateBehavior.RotateIfArrowKeyDown)) {
					return;
				}
			}
			if(dir != 0 && (movement.grounded || airRotate != AirRotateBehavior.RotateAsIfNoInput)) {
				lastDir = dir;
			}
			rotationInDir(lastDir);
		} else {
			var vel = ((movement.endSyncPosition.x - movement.startSyncPosition.x) / movement.syncDelay);
			setAnimatorSpeed(vel);
			

			var from = movement.fromSyncRotation.eulerAngles.y;
			var to = movement.syncRotation.eulerAngles.y;


			//from-to > 0 means try to face left 
			var dir = 	to - from > 0.1 ? -1 :
					to - from < 0.1 ? 1 : 0;
			
			if(Mathf.Abs(to - from) > deltaRotSyncLimit) {
				rotationInDir(dir);
			}
		}

	}

	void rotationInDir(float dir) {
		//low normRot means rotate to leftFacing (dir is -1 if leftFacing)
		//-1 to 0 to 1
		normRot = (Mathf.Clamp(normRot + dir * rotSpeed, -1, 1));
		rigidbody.rotation = Quaternion.Slerp(leftFacingRot, rightFacingRot, (normRot + 1)/2);
	}

	void setAnimatorSpeed(float vel) {
		animator.SetFloat("AbsSpeed", Mathf.Abs(vel));
		animator.SetFloat("Speed", vel);
	}
}
