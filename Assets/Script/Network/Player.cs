using UnityEngine;
using System.Collections;

public class Player : System.Object {
	
	public string playerName;
	public NetworkViewID viewID;
	public NetworkPlayer netPlayer;
	//public Transform playerPrefab;
	public GameObject entity;
	public bool local = false;
	
	public void Instantiate(GameObject prefab, Vector3 pos){
		entity = Network.Instantiate(prefab,pos,Quaternion.identity,0) as GameObject;
		
	}
	
}
