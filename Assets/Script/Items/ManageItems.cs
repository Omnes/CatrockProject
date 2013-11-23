using UnityEngine;
using System.Collections;

public class ManageItems : MonoBehaviour {
	
	public Item[] items;
	
	void Start() {
		var g = GameObject.Find("NetworkItems");
		if(g == null)
			Debug.Log ("could not find gameobject NetworkItems");
		networkItems = g.GetComponent<NetworkItems>();
		if(networkItems == null)
			Debug.Log ("could not find component NetworkItems on gameobject NetworkItems");
	}
	
	void Update () {
		if(Input.GetButtonDown("Weapon0")) {
			Debug.Log ("use weapon 0");
			if(items[(int)Slot.Weapon0])
				items[(int)Slot.Weapon0].use(this);
		}
		if(Input.GetButtonDown("Weapon1")) {
			Debug.Log ("use weapon 1");
			if(items[(int)Slot.Weapon1])
				items[(int)Slot.Weapon1].use(this);
		}
		if(Input.GetButtonDown ("Hat")) {
			Debug.Log ("use hat");
			if(items[(int)Slot.Hat])
				items[(int)Slot.Hat].use(this);
		}
	}
	 
	void OnTriggerStay(Collider other) {
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
		networkView.RPC("AssignNewItemRPC", RPCMode.All, i.id, slot);
	}
	
	[RPC]
	void assignNewItemRPC(int t, Slot slot) {
		items[(int)slot] = networkItems.getItem(t);
	}

}
