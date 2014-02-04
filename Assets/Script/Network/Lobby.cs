using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {

	public int levelToLoad = 0;
	private RobNet robNet;
	private bool connected = false;
	private int defaultPort = 7777;
	private int padding = 32;
	string tempPlayerName = "Player";
	//private float columWidth;
	private Vector2 gridSize = new Vector2(3,3);
	string fieldIP = "localhost";

	// Use this for initialization
	void Start () {
		robNet = GetComponent<RobNet>();
		//columWidth = Screen.width/gridSize.x - padding * 2;
	}
	
	void setConnected(bool con){
		connected = con;
	}
	
	float getColum(int n){
		return Screen.width/gridSize.x*n + padding;
	}
	float getRow(int n){
		return Screen.height/gridSize.y*n + padding;
	}
	float getColumWidth(int n){
		return Screen.width/gridSize.x * n - padding * 2;
	}
	float getRowHeigth(int n){
		return Screen.height/gridSize.y * n - padding * 2;
	}
	Rect makeRect(int columNr,int rowNr,int colWidth,int rowHeight){
		return new Rect(getColum(columNr), getRow(rowNr), getColumWidth(colWidth),getRowHeigth(rowHeight));
	}

	void OnGUI(){

		if(robNet.netState == RobNet.State.Meny){		//bör ligga i andra menyscript igentligen

			GUILayout.BeginArea(makeRect(2,0,1,1));
			GUILayout.BeginVertical();

			tempPlayerName = GUILayout.TextField(tempPlayerName);
			if(GUILayout.Button("Ok")){
				SendMessage("setLocalPlayerName",tempPlayerName);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();

			
			GUILayout.BeginArea(makeRect(0,0,1,3));
			GUILayout.BeginVertical();

			
			if(GUILayout.Button("Host game")){
				robNet.StartServer();
			}
			fieldIP = GUILayout.TextField(fieldIP);
			if(GUILayout.Button("Join lobby")){
				robNet.ConnectToServer(fieldIP,defaultPort);
			}
			if(GUILayout.Button("localhost")){
				robNet.ConnectToServer("127.0.0.1",defaultPort);
			}
			if(GUILayout.Button("Sean")){
				robNet.ConnectToServer("192.168.0.106",defaultPort);
			}
			if(GUILayout.Button("Robin")){
				robNet.ConnectToServer("193.11.160.242",defaultPort);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}else if(robNet.netState == RobNet.State.Lobby){
			drawConnectedPlayers();
			levelChooser();
			chooseEquips();
			
			string[] levels = robNet.levels;
			GUILayout.BeginArea(makeRect(1,2,1,1));
			GUILayout.BeginVertical();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(levels[levelToLoad]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Start Game")){
				Network.RemoveRPCsInGroup(0);
				SendMessage("LoadLevel",levelToLoad);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
		}

	}
	
	private void drawConnectedPlayers(){
		List<Player> connectedPlayers = robNet.connectedPlayers;
		
		GUILayout.BeginArea(makeRect(0,0,1,3));
		GUILayout.BeginVertical();
		GUILayout.Label("your ip: " + Network.player.ipAddress);
		for(int i = 0;i < connectedPlayers.Count; i++){
				
			GUILayout.BeginHorizontal();
			GUILayout.Box(connectedPlayers[i].playerName);
			//int ping = Network.GetAveragePing(Network.connectedPlayers[i].netPlayer);
			//GUILayout.Label("ping: " + ping);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void chooseEquips(){
		NetworkItems netItems = GetComponent<NetworkItems>();
		GUILayout.BeginArea(makeRect(1,0,1,3));
		GUILayout.BeginVertical();
		for(int i = 0;i < netItems.prefabs.Length; i++){

			GUILayout.BeginHorizontal();
			if(GUILayout.Button(""+i)){
				robNet.localPlayer.leftWeaponID = i;
			}
			if(GUILayout.Button(""+i)){
				robNet.localPlayer.hatID = i;
			}
			if(GUILayout.Button(""+i)){
				robNet.localPlayer.rightWeaponID = i;
			}
			GUILayout.FlexibleSpace();
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
		string[] levels = robNet.levels;
		GUILayout.BeginArea(makeRect(2,0,1,3));
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
	
	
	
	
	
}
