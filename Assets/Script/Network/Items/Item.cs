using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	
	public GameObject pickupPrefab;
	public Transform handObject;	

	public enum SlotType {
		Weapon,
		Hat
	}
	 
	public SlotType type;
	public Spell spell;
	
	public int id;
	
	public void onPickup(GameObject owner) {
	}
	
	public void use(GameObject owner, Vector3 direction) {
		spell.cast(owner, direction);
	}
}
