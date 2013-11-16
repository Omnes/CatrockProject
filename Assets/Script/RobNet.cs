using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class RobNet : MonoBehaviour {
	
	public GameObject playerPrefab;
	public Transform spawnPoint;
	
	
	//new
	private List<Player> playerList = new List<Player>();
	public NetworkViewID netviewID;
	private Player localPlayer;

	
	// Use this for initialization
	void Start () {
		//playerList = GetComponent<Lobby>().getPlayers();
		//netviewID = Network.AllocateViewID();
	
	}
	void OnLevelWasLoaded(){
		//spawnPoint = GameObject.Find("Spawnpoint");
		playerList = GetComponent<Lobby>().getPlayers();
		//netviewID = Network.AllocateViewID();
		netviewID = GetComponent<Lobby>().getLocalID();
		Debug.Log("ID OF THIS IS " + netviewID);
		localPlayer = GetComponent<Lobby>().getLocalPlayer();
		
		localPlayer.Instantiate(playerPrefab,spawnPoint.position);
		networkView.RPC("SpawnPlayer",RPCMode.OthersBuffered,netviewID);
		
	}
	
	
	public void SendPlayer(NetworkViewID viewID, Vector3 pos, Quaternion rot, Vector3 moveVec){
		//networkView.RPC("SendPlayerRPC", RPCMode.Others, viewID, pos, rot, moveVec);
	}
	
	
	
	[RPC]
	void SpawnPlayer(NetworkViewID id){
		foreach(Player p in playerList){
			if(p.viewID == id){
				Debug.Log(p.Instantiate(playerPrefab,spawnPoint.position));
			}
		}
	}

}














































