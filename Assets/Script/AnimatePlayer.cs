using UnityEngine;
using System.Collections;


public class AnimatePlayer : MonoBehaviour {
	private Animator animator;

	private float prevVelocity;
	private Quaternion rightRot;
	private Quaternion leftRot;
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
		leftRot = new Quaternion();
		leftRot.eulerAngles = Vector3.up * 90;
		rightRot = new Quaternion();
		rightRot.eulerAngles = Vector3.up * -90;
		leftRotStep = new Quaternion();
		leftRotStep.eulerAngles = Vector3.up * -rotSpeed;
		prevVelocity = 0f;

		movement = GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(movement.isLocal) {
			setAnimatorSpeed(rigidbody.velocity.x);

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
			

			var from = deNormalizeRot(movement.fromSyncRotation.eulerAngles);
			var to = deNormalizeRot(movement.syncRotation.eulerAngles);

			//try to predict direction to rotate based on previous rotation change. 
			var dir = 	to - from > 0.1 ? -1 :
					to - from < 0.1 ? 1 : 0;
			
			if(Mathf.Abs(to - from) > deltaRotSyncLimit) {
				rotationInDir(dir);
			}
		}

	}

	void rotationInDir(float dir) {
		//go from left facing to right facing or vice versa, depending on last input direction.
		normRot = (Mathf.Clamp(normRot + dir * rotSpeed, -1, 1));
		//Debug.Log("normRot " + normRot + " lastDir " + lastDir);
		rigidbody.rotation = Quaternion.Slerp(leftRot, rightRot, (normRot + 1)/2);
	}

	//left rotation becomes negative(0 to -90 to -180) instead of (0 to 270 to 180)
	float deNormalizeRot(Vector3 rot) {
		if(rot.y >= 180) {
			return -(360 - rot.y);
		} else {
			return rot.y;
		}
	}

	void setAnimatorSpeed(float vel) {
		animator.SetFloat("AbsSpeed", Mathf.Abs(vel));
		animator.SetFloat("Speed", vel);
	}
}
