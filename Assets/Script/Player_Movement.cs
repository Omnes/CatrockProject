using UnityEngine;
using System.Collections;

public class Player_Movement : MonoBehaviour {
	
	//private float delayMax = 0.5f;
	//private float delayCounter;
	
	
	public Vector2 maxVelocity = new Vector2(10,10);
	private Vector3 tempVec;
	public float MovementSpeed = 6;

	public bool playerControl = true;
	
	//NYTT
	private Vector3 moveVec = Vector3.zero;
	public Network_Manager theNetwork;
	public NetworkViewID viewID;
	public bool isLocal = true;
	public float slideLimit = 10;
	public float slideSpeed = 10;
	public bool slide = false;
	
	private int raycastCounter = 0;
	
	//teleport
	private float teleCurrentTime;
	public float teleDelayTime = 2f;
	
	//*****CAMERA******
		//camerashake
	private float shakeCount;
	public float shakeTime;
	public float shakeMultiplier;
	public float shakeStrenght;
	
	private bool isShaking;
	public Camera mainCam;
	public float camSpeed;
	
	//camera from screen
	public float cameraDepth;
	public float cameraHeight;
	
	//character controller
	private CharacterController cc;
	
	// Use this for initialization
	void Start () {
		
		teleCurrentTime = Time.time;
		cc = GetComponent<CharacterController>();
		
	}

	
	// Update is called once per frame
	void Update () {
		
		if(isLocal){
			raycastCounter++;
			
			//movevec här måste fixas	#===================#
			//moveVec = new Vector3(0, rigidbody.velocity.y, 0);
			if(playerControl){
				if(!slide){
					if(Input.GetKey(KeyCode.D)){
                        moveVec += Vector3.right * MovementSpeed;
					}
					
					if(Input.GetKey(KeyCode.A)){
                        moveVec += Vector3.left * MovementSpeed;
					}
				}

						//fire shoot
				if(Input.GetMouseButtonDown(0)){
				
					isShaking = true;
					
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					
					if(Physics.Raycast(ray, out hit)){
						//Vector3 fireVec = transform.position - new Vector3(hit.point.x, hit.point.y, transform.position.z);
						//Instantiate(bulletPrefab, new Vector3(0,3,0) + transform.position - (fireVec.normalized * 2), Quaternion.LookRotation(fireVec));
					}
				}
				
				
				//Teleport
				if(Input.GetKey(KeyCode.Space)){
					if(teleCurrentTime + teleDelayTime < Time.time){
						RaycastHit hit;
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if(Physics.Raycast(ray, out hit)){
							transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
							//rigidbody.velocity = new Vector3(0,0,0);
							
						}
						teleCurrentTime = Time.time;
					}
				}
				
				

				if(Input.GetKey(KeyCode.B)){
                    moveVec = new Vector3(500, 100, 0);
					
				}if(Input.GetKey(KeyCode.R)){
                    moveVec = new Vector3(0, 0, 0);
					transform.position = new Vector3(0,5,0);
				}
			}

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
			
			//make sure the rigidbody won't exceed maxVelocity
			//lite finare kod om vi clampar :)
			moveVec.x = Mathf.Clamp(moveVec.x,-maxVelocity.x,maxVelocity.x);
			moveVec.y = Mathf.Clamp(moveVec.y,-maxVelocity.y,maxVelocity.y);
			
			//måste fixa häär	#===================#
			//rigidbody.velocity = moveVec; // att modifiera rigidbody.velocity direkt hela tiden blir väldigt tungt
			moveVec.y -= 20/*gravity*/ * Time.deltaTime;
			
			cc.Move(moveVec * Time.deltaTime);
			
			int n = 1;

            //rigidbody.AddForce(moveVec);
			
			//måste fixa här	#===================#
			//theNetwork.SendPlayer(viewID, transform.position, transform.rotation, rigidbody.velocity);
			
			HandleCamera();
			
		}
		
	}
	
	public void UpdatePlayer(Vector3 pos, Quaternion rot, Vector3 move){
		//måste fixa här	#===================#
       // rigidbody.velocity = move;
        transform.position = pos;
		transform.rotation = rot;
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
	
	void Jump(){
		
		
		
	}
}
