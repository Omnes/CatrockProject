using UnityEngine;
using System.Collections;

using Utility;

public class PlayerHealth : MonoBehaviour {
	
	public bool isLocal = true;
	public float playerHealth = 100.0f;
	public float maxHealth = 100.0f;
	public Texture healthTexture;
	
	private Vector2 healthPos;
	public Vector2 healthPosOffset;
	public Vector2 healthBarSize = new Vector2(1.0f,20.0f);
	
	public bool isDisabled = false;
	
	//		this will get changed later on. fastfix
	private Vector3 locationOfDeath;
	
	//private float respawnTime = 2.0f;
	//private float currentTime;
	
	public GameObject[] playerArray;
	
	// Use this for initialization
	void Start () {
		isLocal = networkView.isMine;
		//hårdkodat ohyeahahea
		healthPosOffset = new Vector2(-50,-15);	
		//playerArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
		//show Health
		healthPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, -transform.position.y, transform.position.z));
		healthPos += healthPosOffset;
		
		//Kill player
		if(playerHealth <= 0 && isDisabled == false){
			Debug.Log("trying to kill");
			TryDisablePlayer();
		}
		
		
		//hårdkodat
		if(playerArray.Length < 2){
			playerArray = GameObject.FindGameObjectsWithTag("Player");
		}
			
		//resurrect all players atm?
		if(isLocal){
			if(Input.GetKey(KeyCode.LeftControl)){
				//forstätt med detta...
				//sök igenom alla playerobjekt och kör message
				
				foreach(GameObject g in playerArray){
					g.SetActive(true);
					g.SendMessage("TryEnablePlayer");
				}
			}	
		}
		
	}
	
	
	
	
	
	
	
	
	//if it is local player, do damage.
	//This function will be executed from elsewhere by SendMessage()
	void TryDoDamage(float damage){
		if(isLocal){//												Är osäker på vilket RPCMode det ska vara här
			networkView.RPC("DoDmg", RPCMode.All, damage);
		}
	}
	
	//tries to disable player. gameObject.SetActive(false)
	void TryDisablePlayer(){
		if(isLocal){
			networkView.RPC("Kill", RPCMode.All);
		}
	}
	
	//tries to enable player. gameObject.SetActive(true)
	void TryEnablePlayer(){
		if(Application.isEditor){
			Debug.Log("trying");
		}
		//if(isDisabled == true){
		//	if(Application.isEditor){
		//		Debug.Log("success");
		//	}
			networkView.RPC("Resurrect", RPCMode.All);
		//}
	}
	
	void OnGUI() {
		GUI.DrawTexture(new Rect(healthPos.x,healthPos.y, playerHealth, healthBarSize.y), healthTexture); //playerHealth as WIDTH IS A BAD THING: CHANGE THIS LATER
	}

	//do damage to playerHealth
	[RPC]
	void DoDmg(float damage){
		playerHealth -= damage;
		if(Application.isEditor){
			Debug.Log("A player has been hit with: "+damage+" damage");
		}
	}
	
	//disable Player
	[RPC]
	void Kill(){
		//kanske itne ska vara här också. kanske ska ta bort denna
		gameObject.SetActive(true);
		
		isDisabled = true;
		locationOfDeath = transform.position;
		gameObject.SetActive(false);
		if(Application.isEditor){
			Debug.Log("A player has been killed");
		}
	}
	
	//enable Player
	[RPC]
	void Resurrect(){
		foreach(GameObject g in playerArray){
			g.SetActive(true);	
		}
		if(Application.isEditor){
			Debug.Log("ACTIVATED");
		}
	}
	
	
	
	/*void Resurrect(){
		gameObject.SetActive(true);
		isDisabled = false;
		playerHealth = maxHealth;
		transform.position = locationOfDeath;
		
		if(Application.isEditor){
			Debug.Log("A player has been ressurected");
		}
	}*/	
}
