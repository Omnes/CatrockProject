using UnityEngine;
using System.Collections;

public class Player : System.Object {
	
	public string playerName;
	public NetworkViewID viewID;
	public NetworkPlayer netPlayer;
	//public Transform playerPrefab;
	public GameObject entity;
	public bool local = false;
	
	public GameObject Instantiate(GameObject prefab, Vector3 pos){
		Debug.Log("OOOOGABOOGAGO");
		GameObject e = (GameObject) GameObject.Instantiate(prefab,pos,Quaternion.identity);
		Debug.Log(e);
		Movement movement = e.GetComponent<Movement>();
		movement.isLocal = local;
		movement.viewID = viewID;
		entity = e;
		return e;
	}
	
}
