using UnityEngine;
using System.Collections;

public class GroundMovementAI : MonoBehaviour {

	private controlAI ctrl;
	private GameObject target;

	private bool inRange = false;
	public float acceleration = 0.1f;
	public float maxSpeed = 10f;
	public float minDistance = 5f;
	public float maxDistance = 10f;
	private Vector3 directionToTarget;

	public enum DistStates {tooClose,inRange,tooFar};
	private DistStates distState;
	
	void Start () {
		ctrl = gameObject.GetComponent<controlAI>();
	}

	void Update () {
		if (ctrl.alertState == controlAI.AlertState.aggroed){
			rangeCheck(); //might not need to check every frame
			Vector3 xdir = new Vector3(directionToTarget.x,0,0);

			if(distState == DistStates.tooFar){
				if(Mathf.Abs(rigidbody.velocity.x) < maxSpeed){
					rigidbody.AddForce(xdir,ForceMode.VelocityChange);
				}
			}
			if(distState == DistStates.tooClose){
				if(Mathf.Abs(rigidbody.velocity.x) < maxSpeed){
					rigidbody.AddForce(-xdir,ForceMode.VelocityChange);
				}
			}
		}
	
	}

	void rangeCheck(){
		float dist = Vector3.Distance(transform.position,target.transform.position);
		directionToTarget = (target.transform.position - transform.position).normalized;
		directionToTarget.z = 0;
		if(dist >= minDistance && dist <= maxDistance){
			distState = DistStates.inRange;
		}else{ 
			if(dist < minDistance){
				distState = DistStates.tooClose;
			}
			if(dist > maxDistance){
				distState = DistStates.tooFar;
			}
		}

	}


	void setTarget(GameObject go){
		target = go;
	}
}
