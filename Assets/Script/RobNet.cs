using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class RobNet : MonoBehaviour {
	
	public GameObject playerPrefab;
	public Transform spawnPoint;
	
	
	//new
	public List<Player> connectedPlayers = new List<Player>();
	public NetworkViewID netviewID;
	public Player localPlayer = new Player();

	
	// Use this for initialization
	void Start () {
		//netviewID = Network.AllocateViewID();
		localPlayer.playerName = "Player";
		localPlayer.netPlayer = Network.player;
		localPlayer.local = true;
	
	}
	void OnLevelWasLoaded(){
		spawnPoint = GameObject.Find("Spawnpoint").transform;
		//connectedPlayers = GetComponent<Lobby>().getPlayers();
		//netviewID = Network.aAllocateViewID();
		netviewID = getLocalID();
		Debug.Log("ID OF THIS IS " + netviewID);
		//localPlayer = GetComponent<Lobby>().getLocalPlayer();
		
		localPlayer.Instantiate(playerPrefab,spawnPoint.position);
		//networkView.RPC("SpawnPlayer",RPCMode.OthersBuffered,netviewID);
		//localPlayer.entity = Network.Instantiate(playerPrefab,spawnPoint.position,Quaternion.identity,0) as GameObject;
		
	}
	
	
	
	[RPC]
	void SpawnPlayer(NetworkViewID id){
		foreach(Player p in connectedPlayers){
			if(p.viewID == id){
				p.Instantiate(playerPrefab,spawnPoint.position);
			}
		}
	}
	
	public void addPlayer(Player p){
		connectedPlayers.Add(p);
	}
	
	public List<Player> getPlayers(){
		return connectedPlayers;
	}
	
	public NetworkViewID getLocalID(){
		return localPlayer.viewID;
	}
	public Player getLocalPlayer(){
		return localPlayer;
	}

}














































