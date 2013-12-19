using UnityEngine;
using System.Collections;

using Utility;

//temporary test of firespell with projectile
public class MeleeSpell : Spell {
	
	private const int distMul = 2;
	
	public GameObject projectilePrefab;
	
/*<<<<<<<OVERHEAD
	public override void cast(GameObject g) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)) {
			var posXY = g.transform.position.XY();
			var dirVector = new Vector2(-1.0f, 0.0f);
			//fireProjectile(Vec.vector3(posXY + dirVector * distMul), dirVector);
			fireProjectile(g, new Vector3(-1.0f, 0.0f, 0.0f));
		}
*/
	
	//colMousePos is the position in the world that a ray cast from casters mouse position hits
	public override void cast(GameObject g, Vector3 colMousePos) {
		var posXYZ = g.transform.position;
		var posXY = g.transform.position.XY();
		var dirVector = (colMousePos.XY() - posXY).normalized;
		meleeProjectile(g, Vec.vector3(dirVector));
		//meleeProjectile(g, new Vector3(-1.0f, 0.0f, 0.0f));
	}
	
	void meleeProjectile(GameObject player, Vector3 direction) {
		var g = (GameObject)Instantiate(projectilePrefab, player.transform.position, transform.rotation);
		var proj = g.GetComponent<Melee_Projectile>();
		proj.dirVec = direction;
		proj.player = player;
	}
}
