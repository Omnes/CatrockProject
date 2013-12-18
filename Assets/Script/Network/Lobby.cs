using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {

	public int levelToLoad = 0;
	private RobNet robNet;
	bool connected = false;
	private int defaultPort = 7777;
	private int padding = 32;
	string tempPlayerName = "Player";
	private float columWidth;

	// Use this for initialization
	void Start () {
		robNet = GetComponent<RobNet>();
		columWidth = Screen.width/3 - padding * 2;
	}
	
	void setConnected(bool con){
		connected = con;
	}

	void OnGUI(){

		if(robNet.netState == RobNet.State.Meny){		//bör ligga i andra menyscript igentligen

			GUILayout.BeginArea(new Rect(Screen.width/3*2 + padding, padding, columWidth ,Screen.height/3 - padding * 2));
			GUILayout.BeginVertical();

			tempPlayerName = GUILayout.TextField(tempPlayerName);
			if(GUILayout.Button("Ok")){
				SendMessage("setLocalPlayerName",tempPlayerName);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();

			
			GUILayout.BeginArea(new Rect(padding, padding, columWidth, Screen.height - padding * 2));
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
		}else if(robNet.netState == RobNet.State.Lobby){
			drawConnectedPlayers();
			levelChooser();
			
			string[] levels = robNet.levels;
			GUILayout.BeginArea(new Rect(Screen.width/3*1 + padding, Screen.height/3*2 + padding, columWidth, Screen.height/3 - padding * 2));
			GUILayout.BeginVertical();

			GUILayout.Label(levels[levelToLoad]);
			if(GUILayout.Button("Start Game")){
				Network.RemoveRPCsInGroup(0);
				networkView.RPC("StartGame",RPCMode.All,levelToLoad);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
		}

	}
	
	private void drawConnectedPlayers(){
		List<Player> connectedPlayers = robNet.connectedPlayers;
		
		GUILayout.BeginArea(new Rect(padding, padding, columWidth, Screen.height - 30));
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
		GUILayout.BeginArea(new Rect(Screen.width/3*2 + padding, padding, columWidth, Screen.height - padding * 2));
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
