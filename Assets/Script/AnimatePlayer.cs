using UnityEngine;
using System.Collections;

public class AnimatePlayer : MonoBehaviour {
	private Animator animator;

	private float prevVelocity;
	private Quaternion rightRot;
	private Quaternion leftRot;
	private Quaternion rightRotStep;
	private Quaternion leftRotStep;
	public float rotSpeed = 5f;
	
	private float normRot = 0f;
	private float lastDir = 0f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		leftRot = new Quaternion();
		leftRot.eulerAngles = Vector3.up * 90;
		rightRot = new Quaternion();
		rightRot.eulerAngles = Vector3.up * -90;
		rightRotStep = new Quaternion();
		rightRotStep.eulerAngles = Vector3.up * rotSpeed;
		leftRotStep = new Quaternion();
		leftRotStep.eulerAngles = Vector3.up * -rotSpeed;

		prevVelocity = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		var dir = 	Input.GetAxis("Horizontal") < 0 ? -1 : 
				Input.GetAxis("Horizontal") > 0 ? 1 : 0;
		if(dir != 0) {
			lastDir = dir;
		}
		
		
		//speed between 0(-1 to -infinity) and 1(1 to infinity)
		normRot = (Mathf.Clamp(normRot + lastDir * rotSpeed, -1, 1));
	       	Debug.Log("normRot " + normRot + " lastDir " + lastDir);	
		rigidbody.rotation = Quaternion.Slerp(leftRot, rightRot, (normRot + 1)/2);
		
		animator.SetFloat("Speed", rigidbody.velocity.x);
		animator.SetFloat("AbsSpeed", Mathf.Abs(rigidbody.velocity.x));
	}
}
