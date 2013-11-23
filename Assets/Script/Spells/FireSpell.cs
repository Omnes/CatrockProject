using UnityEngine;
using System.Collections;

//temporary test of firespell with projectile
public class FireSpell : Spell {
	
	public GameObject projectilePrefab;
	
	public override void cast(GameObject g) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
			Vector3 mousePos = new Vector3(hit.point.x, hit.point.y, transform.position.z);
			Vector3 dirVector = (mousePos - transform.position).normalized;
			
			fireProjectile(g.transform.position + dirVector * 2, dirVector);
		}
	}
	
	void fireProjectile(Vector3 position, Vector3 direction) {
		networkView.RPC("fireProjectileRPC", RPCMode.All, position, direction);
	}
	
	[RPC]
	void fireProjectileRPC(Vector3 position, Vector3 direction) {
		Debug.Log ("just got to shoot a projectile");
		var g = (GameObject)Instantiate(projectilePrefab, position, transform.rotation);
		var proj = g.GetComponent<Fireball_Projectile>();
		proj.dirVec = direction;
	}
}
