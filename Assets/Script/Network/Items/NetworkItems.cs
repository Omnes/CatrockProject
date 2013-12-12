using UnityEngine;
using System.Collections;

public class NetworkItems : MonoBehaviour {
	
	public GameObject[] prefabs;
	
	public Item getItem(int t) {
		return items[t];
	}
	 
	public void Start() {
		items = new Item[prefabs.Length];
		for(var i = 0; i < prefabs.Length; ++i) {
			var item = prefabs[i].GetComponent<Item>();
			item.id = i;
			//cache for fast access during gameplay
			items[i] = item;
		}
	}
	
	private Item[] items;
}
