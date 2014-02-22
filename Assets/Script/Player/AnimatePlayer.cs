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

		if(networkView.isMine) {
			updateCurrentAnimationClip();
			if(hasAnimationClipChanged()) {
				nonLocalPlayCurrentAnimation();
				if(isAnimationClipEnding("Base.IdleJumping") || isAnimationClipEnding("Base.RunJumping")) {
					SendMessage("jumpEnd");
				} else if(isAnimationClipEnding("Base.UsingItemLeft")) {
					OurDebug.Log("using left in movement");
					SendMessage("useItemLeftEnd");
				} else if(isAnimationClipEnding("Base.UsingItemRight")) {
					OurDebug.Log("using right in movement");
					SendMessage("useItemRightEnd");
				}
			}
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
		animator.SetTrigger("AirEnd");
		animator.SetBool("JumpBegin", false);
		animator.SetBool("AirBegin", false);
	}

	public void useItemLeftBegin() {
		animator.SetTrigger("UseItemLeftBegin");
	}

	public void useItemLeftEnd() {
		animator.SetBool("UseItemLeftBegin", false);
	}
	
	public void useItemRightBegin() {
		animator.SetTrigger("UseItemRightBegin");
	}

	public void useItemRightEnd() {
		animator.SetBool("UseItemRightBegin", false);
	}
	
	public void nonLocalPlayCurrentAnimation() {
		networkView.RPC("nonLocalPlayCurrentAnimationRPC", RPCMode.Others, curState.nameHash);
	}


	bool hasAnimationClipChanged() {
		return curState.nameHash != prevState.nameHash;
	}
	bool isAnimationClipBeginning(string s) {
		return curState.IsName(s) && prevState.IsName(s) == false;
	}
	
	bool isAnimationClipEnding(string s) {
		return prevState.IsName(s) && curState.IsName(s) == false;
	}
	
	void newVelocity(Vector3 velocity) {
		syncSpeed = velocity.XZ().magnitude;
	}
	
	void updateCurrentAnimationClip() {
		prevState = curState;
		curState = animator.GetCurrentAnimatorStateInfo(0);
	}

	[RPC]
	void nonLocalPlayCurrentAnimationRPC(int nameHash) {
		OurDebug.Log("Playing anim");
		animator.Play(nameHash);
	}
}
