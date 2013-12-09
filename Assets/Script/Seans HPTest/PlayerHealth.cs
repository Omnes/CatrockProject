using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	
	public bool isLocal = true;
	public float playerHealth = 100.0f;
	public Texture healthTexture;
	
	private Vector2 healthPos;
	public Vector2 healthPosOffset;
	public Vector2 healthBarSize = new Vector2(1.0f,20.0f);
	
	public bool isDisabled = false;
	
	// Use this for initialization
	void Start () {
		isLocal = networkView.isMine;
		//hårdkodat ohyeahahea
		healthPosOffset = new Vector2(-50,-15);
	}
	
	// Update is called once per frame
	void Update () {
		
		healthPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, -transform.position.y, transform.position.z));
		healthPos += healthPosOffset;
		
		//Kill player
		// is this supposed to be local or not ? 
		if(playerHealth <= 0 && isDisabled == false){
			TryDisablePlayer();
		}
	}
	
	//if it is local player, do damage.
	//This function will be executed from elsewhere by SendMessage()
	void TryDoDamage(float damage){
		if(isLocal){//												Är osäker på vilket RPCMode det ska vara här
			networkView.RPC("DoDmg", RPCMode.All, damage);
		}
	}
	
	void TryDisablePlayer(){
		if(isLocal){
			networkView.RPC("Kill", RPCMode.All);
		}
	}
	
	//tryenableplayer ? should try to bring back player to life ? if it is castingtime then maybe player wont be resurrecte4d ?
	void TryEnablePlayer(){
		//networkview.RPC("Resurrect", RPCMode.All);
	}
	
	void OnGUI() {
		GUI.DrawTexture(new Rect(healthPos.x,healthPos.y, playerHealth, healthBarSize.y), healthTexture); //playerHealth as WIDTH IS A BAD THING: CHANGE THIS LATER
	}

	
	[RPC]
	void DoDmg(float damage){
		playerHealth -= damage;
		if(Application.isEditor){
			Debug.Log("A player has been hit with: "+damage+" damage");
		}
	}
	
	//skicka ut namnet på spelaren
	[RPC]
	void Kill(){
		isDisabled = true;
		gameObject.SetActive(false);
		if(Application.isEditor){
			Debug.Log("A player has been killed");
		}
	}
	
	/*[RPC]
	void Resurrect(){
		Debug.Log("A player has been ressurected");
	}*/
	
}
