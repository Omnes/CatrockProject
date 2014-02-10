using UnityEngine;
using System.Collections;

public class GroundMovementAI : MonoBehaviour {

	private controlAI ctrl;
	private GameObject target;

	//private bool inRange = false;
	public float acceleration = 0.1f;
	public float maxSpeed = 10f;
	public float minDistance = 5f;
	public float maxDistance = 10f;
	public float turningSpeed = 5;
	private Vector3 directionToTarget;

	public enum DistStates {tooClose,inRange,tooFar};
	private DistStates distState;
	
	void Start () {
		ctrl = gameObject.GetComponent<controlAI>();
	}
	//just a few functions to make the code more readable
	Vector3 vec3Abs(Vector3 v){
		return new Vector3(Mathf.Abs(v.x),Mathf.Abs(v.y),Mathf.Abs(v.z));
	}

	Vector3 vec3Clamp01(Vector3 v){
		return new Vector3(Mathf.Clamp01(v.x),Mathf.Clamp01(v.y),Mathf.Clamp01(v.z));
	}

	Vector3 vec3Mult(Vector3 v1,Vector3 v2){
		return new Vector3(v1.x*v2.x ,v1.y*v2.y, v1.z*v2.z);
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

	void Update () {
		if (ctrl.alertState == controlAI.AlertState.aggroed){
			rangeCheck(); //might not need to check every frame
			Vector3 dir = new Vector3(directionToTarget.x,0,directionToTarget.z);
			//check if we should add speed on that axis
			Vector3 frommax = new Vector3(maxSpeed,maxSpeed,maxSpeed) - vec3Abs(rigidbody.velocity);
			Vector3 shouldAddSpeed = vec3Clamp01(frommax);
			Vector3 moveVec = vec3Mult(dir, shouldAddSpeed);

			if(distState == DistStates.tooFar){
				rigidbody.AddForce(moveVec*acceleration,ForceMode.VelocityChange);
			}
			if(distState == DistStates.tooClose){
				rigidbody.AddForce(-moveVec*acceleration,ForceMode.VelocityChange);
			}
			//rotation stuff
			float currentDirection = transform.eulerAngles.y;
			if(currentDirection > 179){
				currentDirection -= 360;
			}

			float targetDirection = Mathf.Atan2(moveVec.x,moveVec.z) * Mathf.Rad2Deg;
			float deltaDir = targetDirection - currentDirection;
			if(deltaDir > 180){
				deltaDir -= 360;
			}
			if (Mathf.Abs (deltaDir) > turningSpeed){
				currentDirection += turningSpeed*getSign(deltaDir);
			} else{
				currentDirection = targetDirection;
			}
			transform.rotation = Quaternion.Euler(0,currentDirection,0);
		}
	
	}

	void rangeCheck(){
		float dist = Vector3.Distance(transform.position,target.transform.position);
		directionToTarget = (target.transform.position - transform.position).normalized;
		directionToTarget.y = 0;
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
