using UnityEngine;
using System.Collections;

public class TestProjectilePlayer : MonoBehaviour {
	
	private float currentTime;
	public float delayTime = 2.0f;
	public GameObject projectilePrefab;
	
	// Use this for initialization
	void Start () {
	
		currentTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKey(KeyCode.Space)){
			
			if(currentTime + delayTime < Time.time){
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					
					Vector3 mousePos = new Vector3(hit.point.x, hit.point.y, transform.position.z);
					Vector3 dirVector = mousePos - transform.position;
					
			
					GameObject newProj = (GameObject) Instantiate(projectilePrefab, transform.position, transform.rotation);
					Fireball_Projectile proj = newProj.GetComponent<Fireball_Projectile>();
					proj.dirVec = dirVector.normalized;
					proj.playerPrefab = gameObject;
					
				}
				currentTime = Time.time;
			}
		}
	}
}