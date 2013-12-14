﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {

	public int levelToLoad = 0;
	private RobNet robNet;
	bool connected = false;
	private int defaultPort = 7777;

	// Use this for initialization
	void Start () {
		robNet = GetComponent<RobNet>();
	}
	
	void setConnected(bool con){
		connected = con;
	}
	
	string tempPlayerName = "Player";
	void OnGUI(){

		if(!connected){
			//set player name
			tempPlayerName = GUI.TextField(new Rect(500, 100, 200, 20), tempPlayerName);
			if(GUI.Button(new Rect(500, 130, 40, 40), "Ok")){
				SendMessage("setLocalPlayerName",tempPlayerName);
			}
			
			GUILayout.BeginArea(new Rect(50,30,200,Screen.height - 30));
			GUILayout.BeginVertical();

			
			if(GUILayout.Button("Host game")){
				robNet.StartServer();
			}
			string fieldIP = GUILayout.TextField("localhost");
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
		}else if( Application.loadedLevelName == "lobby"){
			drawConnectedPlayers();
			levelChooser();
			
			string[] levels = robNet.levels;
			GUI.Label(new Rect(400,370, 200,30),levels[levelToLoad]);
			if(GUI.Button(new Rect(400,400, 100,50), "Start Game")){
				Network.RemoveRPCsInGroup(0);
				networkView.RPC("StartGame",RPCMode.All,levelToLoad);
			}
			
		}

	}
	
	private void drawConnectedPlayers(){
		List<Player> connectedPlayers = robNet.connectedPlayers;
		
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
		string[] levels = robNet.levels;
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
	
	
	
	
	
}
