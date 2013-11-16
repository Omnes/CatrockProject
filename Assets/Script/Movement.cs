using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
	//private float delayMax = 0.5f;
	//private float delayCounter;
	
	
	public Vector2 maxVelocity = new Vector2(10,10);
	private Vector3 tempVec;
	public float MovementSpeed = 6;

	public bool playerControl = true;
	
	//NYTT
	private Vector3 moveVec = Vector3.zero;
	public RobNet robNet;
	public NetworkViewID viewID;
	public bool isLocal = true;
	public float slideLimit = 10;
	public float slideSpeed = 10;
	public bool slide = false;
	//network stuff
	private float lastSyncTime = 0f;
	private Vector3 startSyncPosition = Vector3.zero;
	private Vector3 endSyncPosition = Vector3.zero;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	
	private int raycastCounter = 0;
	
	//*****CAMERA******
		//camerashake
		private float shakeCount = 10;
		public float shakeTime = 100;
		public float shakeMultiplier = 10;
		public float shakeStrenght = 200;
		
		private bool isShaking = false;
		public Camera mainCam;
		public float camSpeed = 5;
		
		//camera from screen
		public float cameraDepth = -45;
		public float cameraHeight = 15;

	// Use this for initialization
	void Start () {
		//lastPos = transform.position;
//		delayCounter = Time.time;
		mainCam = Camera.main;
		//robNet = GameObject.Find("Mastermind").GetComponent<RobNet>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(isLocal){
			raycastCounter++;
			moveVec = rigidbody.velocity;
			
			if(playerControl){
				if(!slide){
					if(Input.GetKey(KeyCode.D)){
                        moveVec += Vector3.right * MovementSpeed;
					}
					
					if(Input.GetKey(KeyCode.A)){
                        moveVec += Vector3.left * MovementSpeed;
					}
				}


			}
			moveVec.x = Mathf.Clamp(moveVec.x,-maxVelocity.x,maxVelocity.x);
			moveVec.y = Mathf.Clamp(moveVec.y,-maxVelocity.y,maxVelocity.y);

			//make sure the rigidbody won't exceed maxVelocity
			//lite finare kod om vi clampar :)
			
			
			rigidbody.velocity = moveVec; // att modifiera rigidbody.velocity direkt hela tiden blir väldigt tungt

			HandleCamera();
			
		}else{
			nonLocalUpdate();
		}
		
	}
	
	void nonLocalUpdate(){
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(startSyncPosition,endSyncPosition, syncDelay/syncTime);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if(stream.isWriting){
			
			syncPosition = rigidbody.position;
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
		}else{
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0f; 
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;
			startSyncPosition = rigidbody.position;
			endSyncPosition = syncPosition + syncVelocity * syncDelay;
			
		}
		
	}
	
	
	
	
	
	
	
	void HandleCamera(){
		//player - camera
		Vector2 dirVec = (((cameraHeight * Vector3.up) + transform.position)-mainCam.transform.position)/2;
		Vector3 camPos = mainCam.transform.position;
		
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
		mainCam.transform.position = camPos;
	}
	
	
	void SlideCheck(){
	
		if(raycastCounter % 10 == 0){
			slide = false;
			RaycastHit hit;
			float rayDist = (transform.localScale.y/2f)+0.1f; //ändras beroende på hur modellerna blir sen
			if(Physics.Raycast(transform.position,-Vector3.up,out hit,rayDist)){
				if(Vector3.Angle(hit.normal,Vector3.up) > slideLimit){
					slide = true;
					playerControl = false;
				}
			}
			if(slide){
				Vector3 hitNormal = hit.normal;
				moveVec += new Vector3(hitNormal.x,-hitNormal.y,hitNormal.z)*slideSpeed;
			}else{
				playerControl = true; //får ändra detta sen
			}
		}
	}
}


