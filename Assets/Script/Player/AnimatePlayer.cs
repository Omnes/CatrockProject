using UnityEngine;
using System.Collections;

using Utility;

public class AnimatePlayer : MonoBehaviour {
	Animator animator;
	float syncSpeed = 0;
	
	void Start() {
		animator = GetComponent<Animator>();
	}

	void Update() {
		var speed = networkView.isMine ? rigidbody.velocity.XZ().magnitude : syncSpeed;
		if(networkView.isMine == false && Input.GetKey(KeyCode.Z)) {
			OurDebug.Log("speed " + speed);
		}
		animator.SetFloat("AbsSpeed", speed);
	}

	void newVelocity(Vector3 velocity) {
		syncSpeed = velocity.XZ().magnitude;
	}
}
