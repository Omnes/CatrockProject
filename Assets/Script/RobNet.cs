using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class RobNet : MonoBehaviour {
	
	public GameObject playerPrefab;
	private Transform spawnPoint;
	
	
	//new
	public List<Player> connectedPlayers = new List<Player>();
	public NetworkViewID netviewID;
	public Player localPlayer = new Player();
	
	// Use this for initialization
	void Start () {
		Debug.Log ("starting robnet");
		localPlayer.playerName = "Player";
		localPlayer.netPlayer = Network.player;
		localPlayer.local = true;
	
	}

	void OnNetworkLevelLoaded(){
		var spawnObject = GameObject.Find("Spawnpoint");
		if(spawnObject == null) {
			Debug.Log ("could not find a spawnpoint in the level " + Application.loadedLevelName);
		} else {
			spawnPoint = spawnObject.transform;
			netviewID = getLocalID();
			Debug.Log("ID OF THIS IS " + netviewID);
			localPlayer.Instantiate(playerPrefab, spawnPoint.position);
		}
		
	}
	
	
	/*
	[RPC]
	void SpawnPlayer(NetworkViewID id){
		Debug.Log ("Player spawned"); //does this ever happen?
		
		foreach(Player p in connectedPlayers){
			if(p.viewID == id){
				p.Instantiate(playerPrefab,spawnPoint.position);
			}
		}
	}
	*/
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














































