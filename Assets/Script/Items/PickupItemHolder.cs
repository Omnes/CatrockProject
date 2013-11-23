using UnityEngine;
using System.Collections;

//need to sync over network.
public class PickupItemHolder : MonoBehaviour {
	
	public Item item;
	
	public void assignNewItem(Item item) {
		networkView.RPC("assignNewItemRPC", RPCMode.All, item.id);
	}
	
	private NetworkItems networkItems;
	
	void Start() {
		var g = GameObject.Find("Mastermind");
		networkItems = g.GetComponent<NetworkItems>();
		if(networkItems == null)
			Debug.Log ("could not find component NetworkItems on gameobject NetworkItems");
	}
	
	[RPC]
	private void assignNewItemRPC(int t) {
		item = networkItems.getItem(t);
	}
}
