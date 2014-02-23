using UnityEngine;
using System.Collections;

public class Player : System.Object {
	
	public string playerName;
	public NetworkViewID viewID;
	public NetworkPlayer netPlayer;
	public Transform playerPrefab;
	public GameObject entity;
	public bool local = false;
	public int[] items = {0,1,2};
	public enum ItemSlot {left,right,hat};

	public void SetEquips(int left,int right,int hat){
		items[(int)ItemSlot.left] = left;
		items[(int)ItemSlot.left] = right;
		items[(int)ItemSlot.hat] = hat;
	}
	
	public void Instantiate(GameObject prefab, Vector3 pos){
		entity = Network.Instantiate(prefab,pos,Quaternion.identity,0) as GameObject;
		entity.GetComponent<ManageItems>().setEquips(items[(int)ItemSlot.left],items[(int)ItemSlot.right],items[(int)ItemSlot.hat]);
	}
	
}
