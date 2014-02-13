using UnityEngine;
using System.Collections;

using Utility;

public class AnimatePlayer : MonoBehaviour {
	private Animator animator;
	private float syncSpeed = 0;
	private AnimatorStateInfo curState;
	private AnimatorStateInfo prevState;

	void Start() {
		animator = GetComponent<Animator>();
		if(animator == null) {
			OurDebug.Log("animator could not be found");
			Debug.Break();
		}
		curState = animator.GetCurrentAnimatorStateInfo(0);
	}

	void Update() {
		var speed = networkView.isMine ? rigidbody.velocity.XZ().magnitude : syncSpeed;
		animator.SetFloat("AbsSpeed", speed);

		updateCurrentAnimationClip();
		if(isAnimationClipEnd("Base Layer.IdleJumping") || isAnimationClipEnd("Base Layer.RunJumping")) {
			SendMessage("jumpEnd");
		}
	}

	public void airBegin() {
		animator.SetTrigger("AirBegin");
		animator.SetBool("AirEnd", false);
	}
	
	public void airEnd() {
		animator.SetTrigger("AirEnd");
		animator.SetBool("JumpBegin", false);
		animator.SetBool("AirBegin", false);
	}

	public void jumpBegin() {
		animator.SetTrigger("JumpBegin");
		animator.SetBool("AirEnd", false);
	}

	public void jumpEnd() {
		OurDebug.Log("pls work");
		animator.SetTrigger("AirEnd");
		animator.SetBool("JumpBegin", false);
		animator.SetBool("AirBegin", false);
	}

	bool isAnimationClipBegin(string s) {
		return curState.IsName(s) && prevState.IsName(s) == false;
	}
	
	bool isAnimationClipEnd(string s) {
		return prevState.IsName(s) && curState.IsName(s) == false;
	}

	public void newVelocity(Vector3 velocity) {
		syncSpeed = velocity.XZ().magnitude;
	}

	void updateCurrentAnimationClip() {
		prevState = curState;
		curState = animator.GetCurrentAnimatorStateInfo(0);
	}
}
