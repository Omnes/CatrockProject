using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	
	public bool isLocal = true;
	public float playerHealth = 100.0f;
	public Texture healthTexture;
	
	private Vector2 healthPos;
	public Vector2 healthPosOffset;
	public Vector2 healthBarSize = new Vector2(1.0f,20.0f);
	
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
		
	}
	
	//if it is local player, do damage.
	//This function will be executed from elsewhere by SendMessage()
	void TryDoDamage(float damage){
		if(isLocal){//												Är osäker på vilket RPCMode det ska vara här
			networkView.RPC("DoDamage", RPCMode.All, damage);
		}
	}
	
	void OnGUI() {
		GUI.DrawTexture(new Rect(healthPos.x,healthPos.y, playerHealth, healthBarSize.y), healthTexture); //playerHealth as WIDTH IS A BAD THING: CHANGE THIS LATER
	}

	
	[RPC]
	void DoDamage(float damage){
		playerHealth -= damage;
		Debug.Log("A player has been hit with: "+damage+" damage");
	}
	
}
