using UnityEngine;
using System.Collections;

public class TestProjectilePlayer : MonoBehaviour {
	
	private float currentTime;
	public float delayTime = 2.0f;
<<<<<<< HEAD
	public GameObject projectilePrefab;
	public GameObject meleeProjectile;
=======
	public GameObject fireballProjectile;
	public GameObject thunderProjectile;
	
>>>>>>> f234ce632af3f4388cc27fb0c01deeabeafccffb
	// Use this for initialization
	void Start () {
	
		currentTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKey(KeyCode.Alpha1)){
			
			if(currentTime + delayTime < Time.time){
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					
					Vector3 mousePos = new Vector3(hit.point.x, hit.point.y, transform.position.z);
					Vector3 dirVector = mousePos - transform.position;
					
			
					GameObject newProj = (GameObject) Instantiate(fireballProjectile, transform.position, transform.rotation);
					Fireball_Projectile proj = newProj.GetComponent<Fireball_Projectile>();
					proj.dirVec = dirVector.normalized;
					proj.playerPrefab = gameObject;
					
				}
				currentTime = Time.time;
			}
		}
		
		if(Input.GetKey(KeyCode.Alpha2)){
			
			if(currentTime + delayTime < Time.time){
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					
					Vector3 mousePos = new Vector3(hit.point.x, hit.point.y, transform.position.z);
					Vector3 dirVector = mousePos - transform.position;
					
			
					GameObject newProj1 = (GameObject) Instantiate(thunderProjectile, transform.position, transform.rotation);
					Thunder_Projectile proj1 = newProj1.GetComponent<Thunder_Projectile>();
					proj1.dirVec = dirVector.normalized;
					proj1.playerPrefab = gameObject;
					
					GameObject newProj2 = (GameObject) Instantiate(thunderProjectile, transform.position, transform.rotation);
					Thunder_Projectile proj2 = newProj2.GetComponent<Thunder_Projectile>();
					dirVector = new Vector3(dirVector.x,dirVector.y * 10,dirVector.z);
					proj2.dirVec = dirVector.normalized;
					proj2.playerPrefab = gameObject;
					
					GameObject newProj3 = (GameObject) Instantiate(thunderProjectile, transform.position, transform.rotation);
					Thunder_Projectile proj3 = newProj3.GetComponent<Thunder_Projectile>();
					dirVector = new Vector3(dirVector.x,dirVector.y * 20,dirVector.z);
					proj3.dirVec = dirVector.normalized;
					proj3.playerPrefab = gameObject;
					
				}
				currentTime = Time.time;
			}
		}
		
		
		if(Input.GetKey(KeyCode.Alpha3)){
			
			if(currentTime + delayTime < Time.time){
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					
					Vector3 mousePos = new Vector3(hit.point.x, hit.point.y, transform.position.z);
					Vector3 dirVector = mousePos - transform.position;
					
			
					GameObject newProj = (GameObject) Instantiate(meleeProjectile, transform.position, transform.rotation);
					Melee_Projectile proj = newProj.GetComponent<Melee_Projectile>();
					proj.dirVec = dirVector.normalized;
					
				}
				currentTime = Time.time;
			}
		}
	}
}