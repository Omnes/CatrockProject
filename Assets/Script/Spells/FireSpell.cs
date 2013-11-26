using UnityEngine;
using System.Collections;

using Utility;

//temporary test of firespell with projectile
public class FireSpell : Spell {
	
	private const int distMul = 2;
	
	public GameObject projectilePrefab;
	
	public override void cast(GameObject g) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) {
			var posXY = g.transform.position.XY();
			var dirVector = new Vector2(-1.0f, 0.5f);
			fireProjectile(Vec.vector3(posXY + dirVector * distMul), dirVector);
		}
	}
	
	void fireProjectile(Vector3 position, Vector3 direction) {
		var g = (GameObject)Instantiate(projectilePrefab, position, transform.rotation);
		var proj = g.GetComponent<Fireball_Projectile>();
		proj.dirVec = direction;
	}
}
