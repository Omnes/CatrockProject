using UnityEngine;
using System.Collections;

public class ManageItems : MonoBehaviour {
	
	public Item[] items;
	
	void Start() {
		var g = GameObject.Find("Mastermind");
		if(g == null)
			Debug.Log ("could not find gameobject NetworkItems");
		networkItems = g.GetComponent<NetworkItems>();
		if(networkItems == null)
			Debug.Log ("could not find component NetworkItems on gameobject NetworkItems");
	}
	
	void Update () {
		
		if(networkView.isMine) {
			if(Input.GetButtonDown("Weapon0")) {
				Debug.Log ("use weapon 0");
				useItem(Slot.Weapon0);
			}
			if(Input.GetButtonDown("Weapon1")) {
				Debug.Log ("use weapon 1");
				useItem(Slot.Weapon1);
			}
			if(Input.GetButtonDown ("Hat")) {
				Debug.Log ("use hat");
				useItem(Slot.Hat);
			}
		}
	}
	 
	void OnTriggerStay(Collider other) {
		
		if(networkView.isMine) {
			Debug.Log("every day im triggerin");
			
			if(other.CompareTag("PickupItem")) {
				Debug.Log ("every day im pickuping");
				var pih = other.GetComponent<PickupItemHolder>();
				var item = pih.item;
				
				var slot = Slot.NoSlot;
				
				if(item.type == Item.SlotType.Weapon && Input.GetButtonDown("PickUpWeapon0")) {
					Debug.Log ("attach weapon 0");
					slot = Slot.Weapon0;
				} else if(item.type == Item.SlotType.Weapon && Input.GetButtonDown("PickUpWeapon1")) {
					Debug.Log ("attach weapon 1");
					slot = Slot.Weapon1;
				}
				
				if(item.type == Item.SlotType.Hat && Input.GetButtonDown("PickUpHat")) {
					Debug.Log ("attach hat");
					slot = Slot.Hat;
				}
				
				if(slot != Slot.NoSlot) {
					var oldItem = getItem(slot);
					assignNewItem(item, slot);
					pih.assignNewItem(oldItem);
				}
			}
			
		}
	}
	
	private NetworkItems networkItems;
	
	private enum Slot {
		Weapon0 = 0,
		Weapon1,
		Hat,
		NoSlot
	}
	
	Item getItem(Slot slot) {
		return items[(int)slot];
	}
	
	void assignNewItem(Item i, Slot slot) {
		networkView.RPC("assignNewItemRPC", RPCMode.All, i.id, (int)slot);
	}
	
	void useItem(Slot slot) {
		networkView.RPC("useItemRPC", RPCMode.All, (int)slot);
	}
	
	[RPC]
	void assignNewItemRPC(int t, int slot) {
		items[slot] = networkItems.getItem(t);
	}
	
	[RPC]
	void useItemRPC(int slot) {
		items[slot].use(gameObject);
	}
}
