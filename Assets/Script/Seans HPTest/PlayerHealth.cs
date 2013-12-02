using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	
	public bool isLocal = true;
	public float playerHealth = 100.0f;
	
	
	// Use this for initialization
	void Start () {
		isLocal = networkView.isMine;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//if it is local player, do damage.
	//This function will be executed from elsewhere by SendMessage()
	void TryDoDamage(float damage){
		if(isLocal){//												Är osäker på vilket RPCMode det ska vara här
			networkView.RPC("DoDamage", RPCMode.All, damage);
		}
	}
	
	[RPC]
	void DoDamage(float damage){
		playerHealth -= damage;
		Debug.Log("A player has been hit with: "+damage+" damage");
	}
	
}
