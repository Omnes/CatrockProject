using UnityEngine;
using System.Collections;

using Utility;

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
				OurDebug.Log("use weapon 0");
				useItem(Slot.Weapon0);
			}
			if(Input.GetButtonDown("Weapon1")) {
				OurDebug.Log("use weapon 1");
				useItem(Slot.Weapon1);
			}
			if(Input.GetButtonDown ("Hat")) {
				OurDebug.Log("use hat");
				useItem(Slot.Hat);
			}
		}
	}
	 
	void OnTriggerStay(Collider other) {
		
		if(networkView.isMine) {
			OurDebug.Log("every day im triggerin");
			
			if(other.CompareTag("PickupItem")) {
				OurDebug.Log("every day im pickuping");
				var pih = other.GetComponent<PickupItemHolder>();
				var item = pih.item;
				
				var slot = Slot.NoSlot;
				
				if(item.type == Item.SlotType.Weapon && Input.GetButtonDown("PickUpWeapon0")) {
					OurDebug.Log("attach weapon 0");
					slot = Slot.Weapon0;
				} else if(item.type == Item.SlotType.Weapon && Input.GetButtonDown("PickUpWeapon1")) {
					OurDebug.Log("attach weapon 1");
					slot = Slot.Weapon1;
				}
				
				if(item.type == Item.SlotType.Hat && Input.GetButtonDown("PickUpHat")) {
					OurDebug.Log("attach hat");
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
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) {
			networkView.RPC("useItemRPC", RPCMode.All, (int)slot, hit.point);
		}
	}
	
	[RPC]
	void assignNewItemRPC(int t, int slot) {
		items[slot] = networkItems.getItem(t);
	}
	
	//takes mouse collision point
	[RPC]
	void useItemRPC(int slot, Vector3 colMousePos) {
		items[slot].use(gameObject, colMousePos);
	}
}
