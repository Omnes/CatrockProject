using UnityEngine;
using System.Collections;

public class IceSpell : Spell {
	public GameObject icewall_prefab;
	public override void cast(GameObject g, Vector3 mousePos) {
		Instantiate(icewall_prefab,g.transform.position,Quaternion.identity);
	}
}
