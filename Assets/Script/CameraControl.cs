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
	
	//camera from screen
	public float cameraDepth = -45;
	public float cameraHeight = 15;

	//player.					 MasterMind should assign the correct player to the camera
	public Transform player;
	 
	void Update(){
		HandleCamera();
	}
	
	void HandleCamera(){
		//player - camera
		Vector2 dirVec = (((cameraHeight * Vector3.up) + player.position)-transform.position)/2;
		Vector3 camPos = transform.position;
		
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
		}
		
		if(dirVec.magnitude > 0.1){
			//bestämmer riktningen och hastigheten
			dirVec = dirVec.normalized * camSpeed * dirVec.magnitude * Time.deltaTime;
			//sätter grundvärden för kameran så som höjd och djup
			camPos = new Vector3(camPos.x, camPos.y, cameraDepth);
			camPos += new Vector3(dirVec.x, dirVec.y, 0);
		}
		
		//final cameraposition
		transform.position = camPos;
	}
	
}
