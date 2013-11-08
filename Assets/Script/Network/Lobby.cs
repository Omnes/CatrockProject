using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {
	
	private bool connected = false;
	private int defaultPort = 7777;
	public int maxPlayers = 4;
	public bool isServer = false;
	
	private Player localPlayer = new Player();
	private List<Player> playerList = new List<Player>();

	// Use this for initialization
	void Start () {
		localPlayer.playerName = "Player";
		localPlayer.netPlayer = Network.player;
		
		//Debug.Log (localPlayer.netPlayer.ipAddress);
		
	}
	
	private Vector2 size = new Vector2(75,50);
	private Vector2 startPos = new Vector2(100,100);
	string tempPlayerName = "Player";
	void OnGUI(){

		if(!connected){
			//set player name
			tempPlayerName = GUI.TextField(new Rect(500, 100, 200, 20), tempPlayerName.ToString());
			if(GUI.Button(new Rect(500, 130, 40, 40), "Ok")){
				localPlayer.playerName = tempPlayerName;
			}
			//start a server
			if(GUI.Button(new Rect(100, 100, 250, 75), "Host game")){
				StartServer();
			}
			//join server buttons
			string fieldIP = GUI.TextField(new Rect(100, 300, 200, 20), "localhost");
			if(GUI.Button(new Rect(100, 200, 250,75), "Join lobby")){
				ConnectToServer(fieldIP,defaultPort);
			}
			if(GUI.Button(new Rect(100, 400, 200,50), "localhost")){
				ConnectToServer("127.0.0.1",defaultPort);
			}
			if(GUI.Button(new Rect(100, 460, 200,50), "Sean")){
				ConnectToServer("192.168.0.106",defaultPort);
			}
			if(GUI.Button(new Rect(100, 580, 200,50), "Robin")){
				ConnectToServer("193.11.160.242",defaultPort);
			}
		}else{
			Vector2 offset = Vector2.zero;
			for(int i = 0;i < playerList.Count; i++){
				GUI.Box(new Rect(startPos.x+offset.x,startPos.y+offset.y,size.x,size.y),playerList[i].playerName);
				string ip = playerList[i].netPlayer.ipAddress;
				GUI.Label(new Rect(startPos.x+offset.x + size.x + 10,startPos.y+offset.y,150,size.y),"ip: " + ip);
				offset.y += size.y+5;
			}
			if(GUI.Button(new Rect(400,400, 100,50), "Start Game")){
				ConnectToServer("193.11.160.242",defaultPort);
			}
			
		}

	}
	
	private void StartServer(){
		//!Network.HavePublicAddress() = om NAT behövs eller inte
		Network.InitializeServer(maxPlayers, defaultPort, !Network.HavePublicAddress());
		Debug.Log("Server initializing");
		//OnConnectedToServer();
		//connected = true;
		//localPlayer.viewID = Network.AllocateViewID();
		//playerList.Add(localPlayer);
		isServer = true;
		OnConnectedToServer();
		
	}
	
	private void ConnectToServer(string ip,int port){
		Debug.Log("Connecting to " + ip + ":" + port);
		Network.Connect(ip, port);
	}
	
	void OnFailedToConnect(){
		Debug.Log("Failed connecting to server");
	}
	
	void OnConnectedToServer(){
		Debug.Log ("Connection sucess!");
		connected = true;
		localPlayer.viewID = Network.AllocateViewID();
		//playerList.Add(localPlayer);
		networkView.RPC("NewPlayer",RPCMode.AllBuffered,localPlayer.playerName,localPlayer.viewID,localPlayer.netPlayer);
	}
	
	[RPC]
	private void NewPlayer(string name,NetworkViewID id, NetworkPlayer netplayer){
		Debug.Log("new Player!");
		Player newPlayer = new Player();
		newPlayer.playerName = name;
		newPlayer.viewID = id;
		newPlayer.netPlayer = netplayer;
		newPlayer.local = (id == localPlayer.viewID);
		
		playerList.Add(newPlayer);
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
