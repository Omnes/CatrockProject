using UnityEngine;
using System.Collections;

using Utility;

//temporary test of firespell with projectile
public class FireSpell : Spell {
	
	private const int distMul = 2;
	
	public GameObject projectilePrefab;

	public override void cast(GameObject g, Vector3 direction) {
		fireProjectile(g,direction);
	}
	
	void fireProjectile(GameObject player, Vector3 direction) {

		var pos = player.transform.position;
		var projPos = new Vector3(pos.x, pos.y+5, pos.z) + direction * distMul;
		var g = (GameObject)Instantiate(projectilePrefab, projPos, transform.rotation);
		var proj = g.GetComponent<Fireball_Projectile>();
		proj.dirVec = direction;
		proj.player = player;
	}
}
