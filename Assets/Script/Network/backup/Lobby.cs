using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobbybackup : MonoBehaviour {
	
	private bool connected = false;
	private int defaultPort = 7777;
	public int maxPlayers = 4;
	public bool isServer = false;
	
	private Player localPlayer = new Player();
	private List<Player> connectedPlayers = new List<Player>();
	public string[] levels = {"Johannes_funland", "Robins_funland"};
	public int levelToLoad = 0;
	private RobNet robNet;

	// Use this for initialization
	void Start () {
		robNet = GetComponent<RobNet>();
		localPlayer = robNet.localPlayer;
		DontDestroyOnLoad(gameObject);
		
	}
	
	string tempPlayerName = "Player";
	void OnGUI(){

		if(!connected){
			//set player name
			tempPlayerName = GUI.TextField(new Rect(500, 100, 200, 20), tempPlayerName);
			if(GUI.Button(new Rect(500, 130, 40, 40), "Ok")){
				robNet.localPlayer.playerName = tempPlayerName;
			}
			
			GUILayout.BeginArea(new Rect(50,30,200,Screen.height - 30));
			GUILayout.BeginVertical();

			
			if(GUILayout.Button("Host game")){
				StartServer();
			}
			string fieldIP = GUILayout.TextField("localhost");
			if(GUILayout.Button("Join lobby")){
				ConnectToServer(fieldIP,defaultPort);
			}
			if(GUILayout.Button("localhost")){
				ConnectToServer("127.0.0.1",defaultPort);
			}
			if(GUILayout.Button("Sean")){
				ConnectToServer("192.168.0.106",defaultPort);
			}
			if(GUILayout.Button("Robin")){
				ConnectToServer("193.11.160.242",defaultPort);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}else{
			drawConnectedPlayers();
			levelChooser();
			
			GUI.Label(new Rect(400,370, 200,30),levels[levelToLoad]);
			if(GUI.Button(new Rect(400,400, 100,50), "Start Game")){
				Network.RemoveRPCsInGroup(0);
				networkView.RPC("StartGame",RPCMode.All,levelToLoad);
			}
			
		}

	}
	
	private void drawConnectedPlayers(){
		
		GUILayout.BeginArea(new Rect(50,30,250,Screen.height - 30));
		GUILayout.BeginVertical();
		for(int i = 0;i < connectedPlayers.Count; i++){
				
			GUILayout.BeginHorizontal();
			GUILayout.Box(connectedPlayers[i].playerName);
			int ping = Network.GetAveragePing(connectedPlayers[i].netPlayer);
			GUILayout.Label("ping: " + ping);
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	[RPC]
	public void changeLevelToLoad(int level){
		levelToLoad = level;
	}
	
	private void levelChooser(){
		GUILayout.BeginArea(new Rect(400,30,250,Screen.height - 30));
		GUILayout.BeginVertical();
		for(int i = 0;i < levels.Length; i++){
	
			if(GUILayout.Button(levels[i])){
				levelToLoad = i;
				networkView.RPC("changeLevelToLoad",RPCMode.OthersBuffered,i);
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void StartServer(){
		bool useNAT = !Network.HavePublicAddress();
		Network.InitializeServer(maxPlayers, defaultPort, useNAT);
		Debug.Log("Server initializing");
		isServer = true;
		OnConnectedToServer();
		
	}
	
	private void ConnectToServer(string ip,int port){
		Debug.Log("Connecting to " + ip + ":" + port);
		Network.Connect(ip, port);
	}
	
	void OnFailedToConnect(){
		Debug.Log("Failed Connection");
	}
	
	void OnConnectedToServer(){
		//startServer kallar denaa också, glöm ej!
		Debug.Log ("Connection sucess!");
		connected = true;
		robNet.localPlayer.viewID = Network.AllocateViewID();
		connectedPlayers.Add(localPlayer);
		robNet.addPlayer(localPlayer);
		
		networkView.RPC("NewPlayer",RPCMode.OthersBuffered,localPlayer.playerName,localPlayer.viewID,localPlayer.netPlayer);
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
		robNet.addPlayer(newPlayer);
	}
	
	[RPC]
	private void StartGame(int levelNr){
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(levelToLoad);
		
		Application.LoadLevel(levels[levelNr]);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
		this.enabled = false;
		
	}
	
	
	
}
