using UnityEngine;
using System.Collections;

public class Player : System.Object {
	
	public string playerName;
	public NetworkViewID viewID;
	public NetworkPlayer netPlayer;
	public Transform playerPrefab;
	public GameObject entity;
	public bool local = false;
	public int leftWeaponID = 0;
	public int rightWeaponID = 1;
	public int hatID = 2;

	public void SetEquips(int left,int right,int hat){
		leftWeaponID = left;
		rightWeaponID = right;
		hatID = hat;
	}
	
	public void Instantiate(GameObject prefab, Vector3 pos){
		GameObject thisPlayerPrefab = prefab;
		ManageItems itemManager = thisPlayerPrefab.GetComponent<ManageItems>();
		NetworkItems netItems = GameObject.FindWithTag ("Mastermind").GetComponent<NetworkItems>(); //woah det där kändes inte bra att skriva
		itemManager.items[0] = netItems.getItem(leftWeaponID);
		itemManager.items[1] = netItems.getItem(hatID);
		itemManager.items[2] = netItems.getItem(rightWeaponID);
		entity = Network.Instantiate(thisPlayerPrefab,pos,Quaternion.identity,0) as GameObject;
		entity.GetComponent<ManageItems>().setEquips(leftWeaponID,rightWeaponID,hatID);
	}
	
}
