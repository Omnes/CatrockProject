using UnityEngine;
using System.Collections;

public class FPSEntity : MonoBehaviour {
	
	
	public bool isLocal = true;

	public NetworkViewID viewID;
	
	public SophieNetworkScript theNetwork;
	
	private Vector3 moveVec = Vector3.zero;

	
	private bool grounded;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		if (isLocal){
			
			Vector3 inputVector = Vector3.zero;
			
			if(Input.GetKey(KeyCode.A)){
				inputVector = transform.position + new Vector3(4,0,0) * Time.deltaTime;
				transform.position = transform.position + new Vector3(4,0,0) * Time.deltaTime;
			}
			
			if(Input.GetKey(KeyCode.D)){
				inputVector = transform.position + new Vector3(-4,0,0) * Time.deltaTime;
				transform.position = transform.position + new Vector3(-4,0,0) * Time.deltaTime;
			}
			
			if(Input.GetKey(KeyCode.Space)){
				inputVector = transform.position + new Vector3(0,10,0) * Time.deltaTime;
				transform.position = transform.position + new Vector3(0,10,0) * Time.deltaTime;
			}

			moveVec = inputVector;
			
			theNetwork.SendPlayer(viewID, transform.position, moveVec);
		}
		
	}
	
	public void UpdatePlayer(Vector3 pos, Vector3 move){
		transform.position = pos;
		moveVec = move;
	}
}
