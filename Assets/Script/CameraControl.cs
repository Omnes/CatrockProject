using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	

	//*****CAMERA******
	//camerashake
	private float shakeCount = 10;
	public float shakeTime = 100;
	public float shakeMultiplier = 10;
	public float shakeStrenght = 200;
	
	private bool isShaking = false;
	public float camSpeed = 5;

	public float cameraHeight = 35;
	public float zOffset = 25;
	public float cameraAngle = 65;

	//player
	private Transform mainCamera;

	void Start(){
		mainCamera = Camera.main.transform;
		mainCamera.rotation = Quaternion.Euler(cameraAngle,0,0);
		if(!networkView.isMine){
			this.enabled = false;
		}
	}
	 
	void Update(){
		HandleCamera();
	}
	
	void HandleCamera(){
		//player - camera
		Vector3 targetPosition = (cameraHeight * Vector3.up) + transform.position - zOffset*Vector3.forward;
		Vector3 dirVec = (targetPosition - mainCamera.position)/2;
		Vector3 camPos = mainCamera.position;

		/* disabled for now
		if(isShaking){
			//shakeTime starts
			shakeCount++;
			//the shake
			camPos.x += (Mathf.Sin(Time.time * shakeMultiplier) * 0.1f) * shakeStrenght * Time.deltaTime;
			//camPos.y += (Mathf.Sin(Time.time * shakeMultiplier) * 0.1f) * shakeStrenght * Time.deltaTime;
			
			//if time is up then stop shake
			if(shakeCount >= shakeTime){	
				isShaking = false;
				shakeCount = 0;
			}
		}*/
		
		if(dirVec.magnitude > 0.1){
			//bestämmer riktningen och hastigheten
			dirVec = dirVec.normalized * camSpeed * dirVec.magnitude * Time.deltaTime;
			//sätter grundvärden för kameran så som höjd och djup
			camPos = new Vector3(camPos.x, cameraHeight, camPos.z);
			camPos += new Vector3(dirVec.x,0, dirVec.z);
		}
		
		//final cameraposition
		mainCamera.position = camPos;
	}
	
}
