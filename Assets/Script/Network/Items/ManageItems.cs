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
	public Transform[] itemHandObject = new Transform[3];
	private string[] itemHandObjectJointName = new string[3]{"joint1/joint8/joint9/joint10/joint11", "joint1/joint8/joint12/joint13", "joint1"};

	private enum Slot {
		Weapon0 = 0,
		Weapon1,
		Hat,
		NoSlot
	}
	//items kan nu bli equipade innan Start() körts så var tvungen att flytta upp sakerna. Lämnade sakerna utkommenterade eftersom det inte är min kod
	void Start() {
		/*
		itemHandObject = new Transform[3];
		itemHandObjectJointName = new string[3]{"joint1/joint8/joint9/joint10/joint11", "joint1/joint8/joint12/joint13", "joint1"};
		for(int i = 0; i < 3; ++i) {
			if(items[i] != null) {
				OurDebug.Log("gonna put on start item");
				attachItem(i, items[i]);
			}
		}*/
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
					//hack, because pih can't find networkItems on its own.
					pih.networkItems = networkItems;
					pih.assignNewItem(oldItem);
				}
			}
			
		}
	}
	
	Item getItem(Slot slot) {
		return items[(int)slot];
	}

	public void setEquips(int leftWeapon,int rightWeapon,int hat){
		networkView.RPC("setEquipsRPC", RPCMode.All, leftWeapon, rightWeapon, hat);
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
		var item = networkItems.getItem(t);
		detachItem(slot);
		attachItem(slot, item);
	}

	[RPC]
	void setEquipsRPC(int w1,int w2,int h) {
		tryToGetNetworkItems(); //behöver göra detta innan eftersom rpct körs innan start()
		//detachItem(0);
		attachItem(0, networkItems.getItem(w1));
		//detachItem(1);
		attachItem(1, networkItems.getItem(w2));
		//detachItem(2);
		attachItem(2, networkItems.getItem(h));
	}
	
	void detachItem(int slot) {
		Destroy(itemHandObject[slot].gameObject);
		itemHandObject[slot] = null;
		items[slot] = null;
	}

	void attachItem(int slot, Item item) {
		if(item.handObject == null)
			OurDebug.Log("could not attach null handobject to slot " + slot);
		else
			OurDebug.Log("got item.handObject " + item.handObject + " in slot " + slot);

		var t = ((Transform)Instantiate(item.handObject));
		t.parent = transform.Find(itemHandObjectJointName[slot]);
		t.localPosition = new Vector3(0, 0, 0);
		itemHandObject[slot] = t;
		items[slot] = item;
	}
	
	//takes mouse collision point
	[RPC]
	void useItemRPC(int slot, Vector3 worldDir) {
		OurDebug.Log("using item " + slot + worldDir);
		items[slot].use(gameObject, worldDir);
	}
}
