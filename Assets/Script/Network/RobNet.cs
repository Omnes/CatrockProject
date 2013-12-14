using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobNet : MonoBehaviour {
	
	public GameObject playerPrefab;
	private Transform spawnPoint;
	
	public string[] levels = {"Johannes_funland", "Robins_funland"};
	private bool connected = false;
	private int defaultPort = 7777;
	public int maxPlayers = 4;
	public bool isServer = false;
	
	
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
		DontDestroyOnLoad(gameObject);
	
	}

	void OnLevelWasLoaded(){
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
	public void setLocalPlayerName(string name){
		localPlayer.playerName = name;
	}
	public void setConnected(bool con){
		connected = con;
	}
	
	
	[RPC]
	private void NewPlayer(string name,NetworkViewID id, NetworkPlayer netplayer){
		Debug.Log("new Player!");
		Player newPlayer = new Player();
		newPlayer.playerName = name;
		newPlayer.viewID = id;
		newPlayer.netPlayer = netplayer;
		newPlayer.local = false;
		
		connectedPlayers.Add(newPlayer);
		addPlayer(newPlayer);
	}
	
	[RPC]
	private void StartGame(int levelNr){
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(0); ////_----------------------------------------------------- Fix dis pls      ska vara ny för varje level för att se till att rpcs inte ligger och skräpar
		
		Application.LoadLevel(levels[levelNr]);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
		this.enabled = false;
		
	}
	
	Player findPlayerWithNetPlayer(NetworkPlayer netplayer){
		foreach(Player p in connectedPlayers){
			if(p.netPlayer == netplayer){
				return p;
			}
		}
		return null; // we didnt find it
	}
	
	[RPC]
	public void disconnectPlayer(NetworkPlayer netPlayer){
		Network.RemoveRPCs(netPlayer);
		Network.DestroyPlayerObjects(netPlayer);
		
		Player player = findPlayerWithNetPlayer(netPlayer);
		if(player == null) return; //abort!
		connectedPlayers.Remove(player);
	}
	
	void OnDisconnectedFromServer(){
		SendMessage("setConnected",false);
	}
	
	void OnPlayerDisconnected(NetworkPlayer netPlayer){
		networkView.RPC ("disconnectPlayer",RPCMode.AllBuffered,netPlayer);
	}
	
	public void StartServer(){
		bool useNAT = !Network.HavePublicAddress();
		Network.InitializeServer(maxPlayers, defaultPort, useNAT);
		Debug.Log("Server initializing");
		isServer = true;
		OnConnectedToServer();
		
	}
	
	public void ConnectToServer(string ip,int port){
		Debug.Log("Connecting to " + ip + ":" + port);
		Network.Connect(ip, port);
	}
	
	void OnFailedToConnect(){
		Debug.Log("Failed Connection");
	}
	
	void OnConnectedToServer(){
		//startServer kallar denaa också, glöm ej!
		Debug.Log ("Connection sucess!");
		SendMessage("setConnected",true);
		localPlayer.viewID = Network.AllocateViewID();
		connectedPlayers.Add(localPlayer);
		addPlayer(localPlayer);
		
		networkView.RPC("NewPlayer",RPCMode.OthersBuffered,localPlayer.playerName,localPlayer.viewID,localPlayer.netPlayer);
	}
	
	

}













































