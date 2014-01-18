using UnityEngine;
using System.Collections;

using Utility;

public class PlayerHealth : MonoBehaviour {
	
	public bool isLocal = true;
    //### healthbar ###
    public float playerHealth = 100.0f;
	public float maxHealth = 100.0f;
	public Texture healthTexture;
	private Vector3 healthPos;
	//                                                          hårdkodat!!!
    public Vector2 healthPosOffset = new Vector2(-50.0f, 15.0f);
    //public Vector2 healthPosOffset = new Vector2(0.0f, 0.0f);
	public Vector2 healthBarSize = new Vector2(1.0f,20.0f);

    //### kill and resurrect player ###
	public bool isDisabled = false;
    public GameObject[] playerArray;
	//		                                            this will get changed later on. fastfix!!!
	private Vector3 locationOfDeath;
	
	// Use this for initialization
	void Start () {
		isLocal = networkView.isMine;
        //playerArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

		//show Health
		//Vector3 playerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		healthPos = Camera.main.WorldToScreenPoint(transform.position);
		healthPos = Vec.XY(healthPos) + healthPosOffset;
		healthPos.z = 0.0f;
		
		
		//Kill player
		if(playerHealth <= 0 && isDisabled == false){
			OurDebug.Log("trying to kill");
			TryDisablePlayer();
		}
		
		//                                                      hårdkodat!!!
		if(playerArray.Length < 2){
			playerArray = GameObject.FindGameObjectsWithTag("Player");
		}
			
		//														resurrect all players atm, need to fix this later
		if(isLocal){
			if(Input.GetKey(KeyCode.LeftControl)){
				//activate players on this client
				foreach (GameObject g in playerArray){
					g.SetActive(true);
				}
				//activate player on other clients
				TryEnablePlayer();
			}
			if(Input.GetKey(KeyCode.LeftAlt)){
				playerHealth = -1f;
			}
		}
		
	}
	
	//if it is local player, do damage.
	//This function will be executed from elsewhere by SendMessage()
	void TryDoDamage(float damage){
		if(isLocal){
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
		networkView.RPC("Resurrect", RPCMode.Others);
		OurDebug.Log("Trying to resurrect");
		
	}
	
	
	//try to addforce on target
	void TryDoPush(Vector3 dirVec){
		//																	#lägg in ifsats som frågar om du har invis eller inte
		rigidbody.AddForce(dirVec, ForceMode.VelocityChange);
	}
	
	//new Rect(screenpos.x - 10, Screen.height - (screenpos.y + 10), 20, 20)
	
	
	void OnGUI() {
		GUI.DrawTexture(new Rect(healthPos.x, Screen.height - healthPos.y, playerHealth, healthBarSize.y), healthTexture); //playerHealth as WIDTH IS A BAD THING: CHANGE THIS LATER
	}

	//do damage to playerHealth
	[RPC]
	void DoDmg(float damage){
		playerHealth -= damage;
	}
	
	//disable Player
	[RPC]
	void Kill(){
		isDisabled = true;
		locationOfDeath = GameObject.Find("Spawnpoint").transform.position;
		gameObject.SetActive(false);
		
		OurDebug.Log("RPC : Killing");
	}
	
	//enable Player
	[RPC]
	void Resurrect(){
		foreach(GameObject g in playerArray){	
			g.SetActive(true);
			if(g.GetComponent<PlayerHealth>().isDisabled){
				g.networkView.RPC("WakeUp", RPCMode.All);
			}
		}
		OurDebug.Log("RPC : Resurrecting");
		
	}
	
	//Måste detta vara ett RPC ?
	//väcker spelaren och säger till alla att han har fullt liv. kanske ska ligga i [RPC]Resurrect
	[RPC]
	void WakeUp(){
		isDisabled = false;
		playerHealth = maxHealth;
		transform.position = locationOfDeath;
		
		OurDebug.Log("IMALIVE");
    }
}
