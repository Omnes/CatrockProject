using UnityEngine;
using System.Collections;

using Utility;

//temporary test of firespell with projectile
public class FireSpell : Spell {
	
	private const int distMul = 2;
	
	public GameObject projectilePrefab;
	
	//colMousePos is the position in the world that a ray cast from casters mouse position hits
	public override void cast(GameObject g, Vector3 colMousePos) {
		var posXYZ = g.transform.position;
		var posXY = g.transform.position.XY();
		var dirVector = (colMousePos.XY() - posXY).normalized;
		fireProjectile(posXYZ + Vec.vector3(dirVector * distMul), Vec.vector3(dirVector));
	}
	
	void fireProjectile(Vector3 position, Vector3 direction) {
		var g = (GameObject)Instantiate(projectilePrefab, position, transform.rotation);
		var proj = g.GetComponent<Fireball_Projectile>();
		proj.dirVec = direction;
	}
}
