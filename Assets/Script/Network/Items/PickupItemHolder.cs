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
		tryToFindNetworkItems();
	}

	void Update() {
		tryToFindNetworkItems();
	}

	void onLevelWasLoaded() {
		//mastermind is dead, try to bring it back
		networkItems = null;
		tryToFindNetworkItems();
	}

	GameObject findMastermindHack() {
		GameObject g;
		g = GameObject.Find("Mastermind");
		if(g == null)
			g = GameObject.Find("Mastermind(Clone)");
		return g;
	}

	void tryToFindNetworkItems() {
		if(networkItems == null) {
			var g = GameObject.FindWithTag("Mastermind");
			if(g == null); {
				return;
			}
			networkItems = g.GetComponent<NetworkItems>();
			if(networkItems == null) {
			}
		}
	}

	[RPC]
	private void assignNewItemRPC(int t) {
		tryToFindNetworkItems();
		if(networkItems == null) {
			OurDebug.Log("could not find networkItems when assignNewItemRPC needed them");
			//shouldn't ever happen, because of hack solution in ManageItems.
			Debug.Break();
			return;
		}
		item = networkItems.getItem(t);
	}
}
