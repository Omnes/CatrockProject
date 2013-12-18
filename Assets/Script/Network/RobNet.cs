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
	private int levelPrefix = 0;

	public enum State{Meny,Lobby,Ingame};
	public State netState = State.Meny;

	
	
	//new
	public List<Player> connectedPlayers = new List<Player>();
	public NetworkViewID netviewID;
	public Player localPlayer = new Player();
	
	// Use this for initialization
	void Start () {
		localPlayer.playerName = "Player";
		localPlayer.netPlayer = Network.player;
		localPlayer.local = true;
		DontDestroyOnLoad(gameObject);
	
	}

	void OnLevelWasLoaded(){
		//spawn the local player when the level is loaded
		var spawnObject = GameObject.Find("Spawnpoint");
		if(spawnObject == null) {
			Debug.Log ("Could not find a spawnpoint in the level " + Application.loadedLevelName);
		} else {
			spawnPoint = spawnObject.transform;
			netviewID = getLocalID();
			localPlayer.Instantiate(playerPrefab, spawnPoint.position);
		}
		
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
	private void NewPlayer(string name, NetworkViewID id, NetworkPlayer netplayer){
		Debug.Log("New Player!");
		Player newPlayer = new Player();
		newPlayer.playerName = name;
		newPlayer.viewID = id;
		newPlayer.netPlayer = netplayer;
		newPlayer.local = false;
		connectedPlayers.Add(newPlayer);
	}
	
	[RPC]
	private void StartGame(int levelNr){
		// disable rpcs -> change prefix for rpcs -> loadlevel -> enable rpcs
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(levelPrefix);
		levelPrefix++;
		netState = State.Ingame;
		Application.LoadLevel(levels[levelNr]);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
		
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
		netState = State.Lobby;
		
		networkView.RPC("NewPlayer",RPCMode.OthersBuffered,localPlayer.playerName,localPlayer.viewID,localPlayer.netPlayer);
	}
	
	

}













































