using UnityEngine;
using System.Collections;

public class Player_movement : MonoBehaviour {
	
	public Vector2 maxVelocity;
	private float currentTime;
	public float delayTime;
	public GameObject bulletPrefab;
	
	
	//camerashake
	private float shakeCount;
	public float shakeTime;
	public float shakeMultiplier;
	public float shakeStrenght;
	
	private bool isShaking;
	public Camera mainCam;
	public float camSpeed;
	//public float camAcc;
	
	//camera from screen
	public float cameraDepth;
	public float cameraHeight;
	
	// Use this for initialization
	void Start () {
		currentTime = Time.time;
		//camershake
		isShaking = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKey(KeyCode.A)){
			
			transform.position += new Vector3(-0.1f,0,0);
			
		}
		if(Input.GetKey(KeyCode.D)){
			
			transform.position += new Vector3(0.1f,0,0);
			
		}
		
		if(Input.GetKey(KeyCode.Space)){
			
			if(currentTime + delayTime < Time.time){
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
					rigidbody.velocity = new Vector3(0,0,0);
					
				}
				currentTime = Time.time;
			}
		}
		
		//fire shoot
		if(Input.GetMouseButtonDown(0)){
		
			isShaking = true;
			
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit)){
				Vector3 fireVec = transform.position - new Vector3(hit.point.x, hit.point.y, transform.position.z);
				Instantiate(bulletPrefab, new Vector3(0,3,0) + transform.position - (fireVec.normalized * 2), Quaternion.LookRotation(fireVec));
			}
		}
		
		//clamp and such. mveoment things
		Vector3 moveVec = transform.rigidbody.velocity;
		
		moveVec.x = Mathf.Clamp(moveVec.x,-maxVelocity.x,maxVelocity.x);
		moveVec.y = Mathf.Clamp(moveVec.y,-maxVelocity.y,maxVelocity.y);
		
		rigidbody.velocity = moveVec;
		
		
		//camerashake
		//test
		HandleCamera();
		
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
	
	void OnCollisionEnter(Collision other){
		
		if(other.collider.tag == "FriendlyBullet"){
			
			rigidbody.velocity +=other.rigidbody.velocity;
			
		}
	}
	
}
