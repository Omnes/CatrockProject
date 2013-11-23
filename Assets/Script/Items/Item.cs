using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	
	public GameObject pickupPrefab;
	
	public enum SlotType {
		Weapon,
		Hat
	}
	 
	public SlotType type;
	public Spell spell;
	public string apperance;
	public string castAnimation;
	
	public int id;
	
	public void onPickup(Component owner) {
		//give owner a fancy hat or something
		Debug.Log("apperance changed to " + apperance);
	}
	
	public void use(Component owner) {
		Debug.Log("player does an animation " + castAnimation);
		spell.cast(owner.gameObject);
	}
}