using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SophieNetworkScript : MonoBehaviour {
	
	
	private bool isServer = false;
	private string gameName = "TestSession";
	
	
	private bool connected = false;
	
	public GameObject fpsEntityPrefab;
	
	private List<FPSEntity> fpsEntities = new List<FPSEntity>();
	
	public NetworkViewID netviewID;
	
	public string playerName = "DefaultPlayerName";

	
	public void SendPlayer(NetworkViewID viewID, Vector3 pos, Vector3 moveVec){
		networkView.RPC("SendPlayerRPC", RPCMode.Others, viewID, pos, moveVec);
	}
	
	[RPC]
	void SendPlayerRPC(NetworkViewID viewID, Vector3 pos, Vector3 moveVec){
		for (int i=0; i<fpsEntities.Count; i++){
			if (viewID == fpsEntities[i].viewID){
				fpsEntities[i].UpdatePlayer(pos, moveVec);
			}
		}
	}
	
	void InstantiateFPSEntity(bool isLocal, NetworkViewID anID){
		GameObject newEntity = (GameObject)GameObject.Instantiate(fpsEntityPrefab);
		FPSEntity entity = newEntity.GetComponent<FPSEntity>();
		entity.theNetwork = this;
		
		if (isLocal){
			entity.viewID = netviewID;
			entity.isLocal = true;
		}else{
			entity.viewID = anID;
			entity.isLocal = false;
		}
		fpsEntities.Add(entity);
	}
	
	void OnGUI(){
		if (!connected){
			
			
			
			
			//set up server
			gameName = GUILayout.TextField(gameName);
			if (GUILayout.Button("Init Server")){
				bool useNat = !Network.HavePublicAddress();
				Debug.Log("Initialising server, has public address: " + Network.HavePublicAddress().ToString());
				Network.InitializeServer(32,25000, useNat);
				
			}
			
			GUILayout.Label("~~~~~~~~~");
			//connect to a server
			
			if (GUILayout.Button("Request Host List")){
				MasterServer.RequestHostList("SophiesTestGame");
			}
			HostData[] hostData = MasterServer.PollHostList();
			
			for (int i=0; i<hostData.Length; i++){
				GUILayout.BeginHorizontal();
				GUILayout.Label(" - " + hostData[i].gameName);
				if (GUILayout.Button("Connect")){
					Network.Connect(hostData[i]);
				}
				GUILayout.EndHorizontal();
			}
			
			
			
			GUILayout.Label("~~~~~~~~~");
		
		}
		
			
		
		if (connected){
			if (GUILayout.Button("Disconnect Server")){
				
				
				if (!isServer){
					networkView.RPC("PlayerLeave", RPCMode.OthersBuffered, netviewID);
				}else{
					networkView.RPC("ServerLeave", RPCMode.OthersBuffered);
				}
				
				
				Network.Disconnect();
				if (isServer) MasterServer.UnregisterHost();
				connected = false;
				isServer = false;
				
				Camera.main.transform.parent = null;
				for (int i=0; i<fpsEntities.Count; i++){
					Destroy(fpsEntities[i].gameObject);
				}
				fpsEntities = new List<FPSEntity>();
			}
			
			if (isServer){
				GUILayout.Label("You are Host");
			}else{
				GUILayout.Label("You are Client");
			}
			
		}
		
		
		
		
	}
	
	[RPC]
	void NewPlayer(NetworkViewID viewID, string name){
		InstantiateFPSEntity(false, viewID);
	}
	
	
	void OnConnectedToServer(){
		connected = true;
		Debug.Log("Connected to a server");
		
		netviewID = Network.AllocateViewID();
		InstantiateFPSEntity(true, netviewID);
		networkView.RPC("NewPlayer", RPCMode.OthersBuffered, netviewID, playerName);
		
	}
	
	void OnServerInitialized() {
        Debug.Log("Server initialized, now registering...");
		
		MasterServer.RegisterHost("SophiesTestGame", gameName);
    }
	
	
    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.RegistrationSucceeded){
			//success registering
			
			if (!connected){
				netviewID = Network.AllocateViewID();
				InstantiateFPSEntity(true, netviewID);
				networkView.RPC("NewPlayer", RPCMode.OthersBuffered, netviewID, playerName);
			}
			
			isServer = true;
			connected = true;
            Debug.Log("Server registered");
			
		}else if (msEvent == MasterServerEvent.RegistrationFailedNoServer || msEvent == MasterServerEvent.RegistrationFailedGameType || msEvent == MasterServerEvent.RegistrationFailedNoServer){
			//failure registering
			Debug.Log("server registration failed, disconnecting");
			Network.Disconnect();
			
		}
    }

	
	
	void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);
    }
	
	void OnApplicationQuit(){
		if (connected){
			if (!isServer){
				networkView.RPC("PlayerLeave", RPCMode.OthersBuffered, netviewID);
			}else{
				networkView.RPC("ServerLeave", RPCMode.OthersBuffered);
			}
			Network.Disconnect();
			if (isServer) MasterServer.UnregisterHost();
		}
	}
	
	[RPC]
	void PlayerLeave(NetworkViewID viewID){
		Debug.Log("A player did one");
		for (int i=0; i<fpsEntities.Count; i++){
			if (fpsEntities[i].viewID == viewID){
				Destroy(fpsEntities[i].gameObject);
				fpsEntities.RemoveAt(i);
			}
		}
	}
	
	[RPC]
	void ServerLeave(){
		Debug.Log("THE SERVER BUGGERED OFF! HOW RUDE D:");
		
		Network.Disconnect();
		if (isServer) MasterServer.UnregisterHost();
		connected = false;
		isServer = false;

		for (int i=0; i<fpsEntities.Count; i++){
			Destroy(fpsEntities[i].gameObject);
		}
		fpsEntities = new List<FPSEntity>();
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
