using UnityEngine;
using System.Collections;


//vatvat ? for List
using System.Collections.Generic;

public class Network_Manager : MonoBehaviour {
	
	private string NameIP = "127.0.0.1";//"192.168.0.106"; //193.0.105.25
	//private string NameIP = "Enter IP";//"192.168.0.106"; //193.0.105.25
//	private int NamePORT = 7777; //Whatever port you have open. 7777?
		
	public GameObject playerPrefab;
	public GameObject spawnObj;
	
	
	//new
	private List<Player_Movement> playerList = new List<Player_Movement>();
	public NetworkViewID netviewID;

	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SendPlayer(NetworkViewID viewID, Vector3 pos, Quaternion rot, Vector3 moveVec){
		networkView.RPC("SendPlayerRPC", RPCMode.Others, viewID, pos, rot, moveVec);
	}
	
	[RPC]
	void SendPlayerRPC(NetworkViewID viewID, Vector3 pos, Quaternion rot, Vector3 moveVec){
		for (int i=0; i<playerList.Count; i++){
			if (viewID == playerList[i].viewID){
				playerList[i].UpdatePlayer(pos, rot, moveVec);
			}
		}
	}
	
	
	void OnGUI(){

			if(!Network.isClient && !Network.isServer){
				
				if(GUI.Button(new Rect(100, 100, 250, 75), "Start Server")){
					
					StartServer();
					
				}
				
				if(GUI.Button(new Rect(100, 200, 250,75), "Join Server")){
					
					ConnectToServer();
					
				}
				if(GUI.Button(new Rect(100, 400, 200,50), "localhost")){
					NameIP = "127.0.0.1";
					ConnectToServer();
					
				}
				if(GUI.Button(new Rect(100, 460, 200,50), "192.168.0.106")){
					NameIP = "192.168.0.106";
					ConnectToServer();
					
				}
				if(GUI.Button(new Rect(100, 520, 200,50), "193.0.105.25")){
					NameIP = "193.0.105.25";
					ConnectToServer();
					
				}
				if(GUI.Button(new Rect(100, 520, 200,50), "193.11.160.242")){
					NameIP = "193.11.160.242";
					ConnectToServer();
					
				}
			
			//NameIP = GUI.TextField(new Rect(100, 400, 200, 20), NameIP.ToString());
			
			}	

	}
	
	//new
	[RPC]
	void NewPlayer(NetworkViewID viewID){
		
		SpawnPlayer(false, viewID);
		
	}
	
	
	//***CREATE PLAYER***
	
	void SpawnPlayer(bool isLocal, NetworkViewID anID){

		//skapar spelaren
		GameObject newEntity = (GameObject) GameObject.Instantiate(playerPrefab);
		Player_Movement entity = newEntity.GetComponent<Player_Movement>();
		//ger network
		entity.theNetwork = this;
		
		
		if (isLocal){
			entity.viewID = netviewID;
			entity.isLocal = true;
		}else{
			entity.viewID = anID;
			entity.isLocal = false;
		}
		
		
		playerList.Add(entity);
		
	}
	
	
	//***JOIN SERVER***
	
	private void ConnectToServer(){
		
		Network.Connect(NameIP, 7777);
		Debug.Log("X" + NameIP + "X");
		
	}
	
	//unity default
	void OnConnectedToServer(){
		
		netviewID = Network.AllocateViewID();
		
		//Skapar ny spelare efter Connect
		SpawnPlayer(true, netviewID);
		networkView.RPC("NewPlayer", RPCMode.OthersBuffered, netviewID);
		
	}
	
	//FAILED TO CONNECT
	void OnFailedToConnect(){
		
		Debug.Log("Failed to connect to: " + NameIP);
		
	}
	
	//****SERVERTHINGYS****

	//START SERVER
	private void StartServer(){
		//!Network.HavePublicAddress() = I don't know
		Network.InitializeServer(5, 7777, !Network.HavePublicAddress());
		
	}
	
	//unity default																//HÄR?
	void OnServerInitialized(){
		
	}
	
	//DISCONNECT
	void OnPlayerDisconnected(NetworkPlayer player){
		
		Debug.Log("Cleanup player: " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);

	}	
}
