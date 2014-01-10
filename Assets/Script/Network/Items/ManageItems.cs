using UnityEngine;
using System.Collections;

using Utility;

public class ManageItems : MonoBehaviour {
	
	public Item[] items;
	public float rotSpeed;

	private float schedTime;
	
	private NetworkItems networkItems;
	
	private Slot schedSlot;
	private Quaternion schedBegRot;
	private Quaternion schedEndRot;
	public bool schedDone = true;
	private float schedCurTime = 0;

	private enum Slot {
		Weapon0 = 0,
		Weapon1,
		Hat,
		NoSlot
	}

	void Start() {
	
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
		if(networkItems == null) {
			return false;
		} else {
			return true;
		}
	}

	void Update () {
		
		if(schedDone == false) {
			schedCurTime += Time.deltaTime;
			transform.rotation = Quaternion.Lerp(schedBegRot, schedEndRot, Mathf.Clamp(schedCurTime / schedTime, 0, 1));
			if(schedCurTime >= schedTime) {
			  	SendMessage("itemAnimationEnd");
			}
		}

		if(networkView.isMine && tryToGetNetworkItems() == true) {
			if(Input.GetButtonDown("Weapon0")) {
				OurDebug.Log("use weapon 0");
				scheduleUseItem(Slot.Weapon0);
			}
			if(Input.GetButtonDown("Weapon1")) {
				OurDebug.Log("use weapon 1");
				scheduleUseItem(Slot.Weapon1);
			}
			if(Input.GetButtonDown ("Hat")) {
				OurDebug.Log("use hat");
				scheduleUseItem(Slot.Hat);
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
	
	Item getItem(Slot slot) {
		return items[(int)slot];
	}
	
	void assignNewItem(Item i, Slot slot) {
		networkView.RPC("assignNewItemRPC", RPCMode.All, i.id, (int)slot);
	}
	
	void scheduleUseItem(Slot slot) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
            	if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
			var xzdir = hit.point.XZ() - transform.position.XZ();
			var dir = new Vector3(xzdir.x, 0, xzdir.y).normalized;
			
			rigidbody.velocity = new Vector3(0, 0, 0);
			schedSlot = slot;
			schedBegRot = transform.rotation;
			schedEndRot = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.z), 0);

			var deltaEuler = Quaternion.Angle(schedBegRot, schedEndRot);
			schedTime = deltaEuler / rotSpeed;
			schedCurTime = 0;
			SendMessage("itemAnimationBegin");
		}
	}

	void useItem(Slot slot) {
		networkView.RPC("useItemRPC", RPCMode.All, (int)slot, transform.forward);
	}

	void itemAnimationBegin() {
		schedDone = false;
	}

	void itemAnimationEnd() {
		schedDone = true;
		useItem(schedSlot);
	}

	[RPC]
	void assignNewItemRPC(int t, int slot) {
		items[slot] = networkItems.getItem(t);
	}
	
	//takes mouse collision point
	[RPC]
	void useItemRPC(int slot, Vector3 worldDir) {
		OurDebug.Log("using item " + slot + worldDir);
		items[slot].use(gameObject, worldDir);
	}
}
