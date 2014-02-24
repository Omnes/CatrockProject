using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class ManageItems : MonoBehaviour {
	
	public Item[] items;
	public Transform[] itemHandObject = new Transform[3];
	public string[] itemHandObjectJointName = new string[3]{"R_hand_joint","L_hand_joint","head_joint"};

	private bool nextItemSwap = false;	//toggles between picking up to left or right
	private NetworkItems networkItems;

	void OnTriggerStay(Collider other) {
		if(networkView.isMine && other.CompareTag("PickupItem") && Input.GetButtonDown("PickUpItem")) {
			var pih = other.GetComponent<PickupItemHolder>();
			var item = pih.item;
			var slot = slotforPickUp(item);
			
			if(slot != Slot.NoSlot) {
				var oldItem = getItem(slot);
				assignNewItem(item, slot);
				//hack, because pih can't find networkItems on its own.
				pih.networkItems = networkItems;
				pih.assignNewItem(oldItem);
			}
		}
	}
	
	public void setEquips(int leftWeapon, int rightWeapon, int hat){
		networkView.RPC("setEquipsRPC", RPCMode.All, leftWeapon, rightWeapon, hat);
	}

	public void useItemLeftEnd() {
		useItem(Slot.Left);
	}

	public void useItemRightEnd() {
		useItem(Slot.Right);
	}
	
	void assignNewItem(Item i, Slot slot) {
		networkView.RPC("assignNewItemRPC", RPCMode.All, i.id, (int)slot);
	}

	void attachItem(int slot, Item item) {
		if(item.handObject == null)
			OurDebug.Log("could not attach null handobject to slot " + slot);
		else
			OurDebug.Log("got item.handObject " + item.handObject + " in slot " + slot);

		var t = ((Transform)Instantiate(item.handObject));
		t.parent = findChild(transform,itemHandObjectJointName[slot]);
		t.localPosition = new Vector3(0, 0, 0);
		itemHandObject[slot] = t;
		items[slot] = item;
	}

	void detachItem(int slot) {
		Destroy(itemHandObject[slot].gameObject);
		itemHandObject[slot] = null;
		items[slot] = null;
	}

	//rekursive search for child name
	Transform findChild(Transform parent,string target){
		if(parent.name == target){
			return parent;
		}
		foreach(Transform child in parent){
			Transform res = findChild(child,target);
			if(res != null){
				return res;
			}
		}
		return null;
	}

	Item getItem(Slot slot) {
		return items[(int)slot];
	}

	private enum Slot {
		Left = 0,
		Right,
		Hat,
		NoSlot
	}

	Slot slotforPickUp(Item item) {
			if(item.type == Item.SlotType.Hat) {
					return Slot.Hat;
			} else {
					nextItemSwap = !nextItemSwap;
					return nextItemSwap ? Slot.Left : Slot.Right;
			}
	}

	bool tryToGetNetworkItems() {
		if(networkItems != null) {
			return true;
		}
		
		var g = GameObject.FindWithTag("Mastermind");
		if(g == null) {
			OurDebug.Log("Could not find Mastermind");
			return false;
		}
		
		networkItems = g.GetComponent<NetworkItems>();
		return networkItems != null;
	}

	void useItem(Slot slot) {
		networkView.RPC("useItemRPC", RPCMode.All, (int)slot, transform.forward);
	}

	[RPC]
	void assignNewItemRPC(int t, int slot) {
		var item = networkItems.getItem(t);
		detachItem(slot);
		attachItem(slot, item);
	}
	
	[RPC]
	void setEquipsRPC(int w1,int w2,int h) {
		tryToGetNetworkItems(); //behöver göra detta innan eftersom rpct körs innan start()
		attachItem(0, networkItems.getItem(w1));
		attachItem(1, networkItems.getItem(w2));
		attachItem(2, networkItems.getItem(h));
	}

	//takes mouse collision point
	[RPC]
	void useItemRPC(int slot, Vector3 worldDir) {
		OurDebug.Log("using item " + slot + worldDir);
		items[slot].use(gameObject, worldDir);
	}
}
