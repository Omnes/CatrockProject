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
	
	// Use this for initialization
	void Start () {
		//lastPos = transform.position;
//		delayCounter = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if(isLocal){
			raycastCounter++;
			moveVec = new Vector3(0,rigidbody.velocity.y,0);
			if(playerControl){
				if(!slide){
					if(Input.GetKey(KeyCode.D)){
                        moveVec += Vector3.right * MovementSpeed;
					}
					
					if(Input.GetKey(KeyCode.A)){
                        moveVec += Vector3.left * MovementSpeed;
					}
				}
				
				if(Input.GetKey(KeyCode.Space)){
					
					
					//well.. this doesnt work atm. The main camera is prob not the camera we want to use
					Vector2 mp = Input.mousePosition;
					Vector3 tempPos = Camera.main.ScreenToWorldPoint(new Vector3(mp.x,mp.y,transform.position.z));
					transform.position = new Vector3(tempPos.x,tempPos.y, transform.position.z);

					slide = false;
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
			
			rigidbody.velocity = moveVec; // att modifiera rigidbody.velocity direkt hela tiden blir väldigt tungt
			int n = 1;

            //rigidbody.AddForce(moveVec);

			theNetwork.SendPlayer(viewID, transform.position, transform.rotation, rigidbody.velocity);
			
		}
		
	}
	
	public void UpdatePlayer(Vector3 pos, Quaternion rot, Vector3 move){
        rigidbody.velocity = move;
        transform.position = pos;
		transform.rotation = rot;
	}
	
}
