using UnityEngine;
using System.Collections;

public class Fireball_Projectile : MonoBehaviour {
	
	public Vector3 dirVec;					//directionvektor from player
	public GameObject playerPrefab;			//must get creator from creator to not collide with creator
	public float Speed = 5.0f;
	public GameObject ExploPrefab;
	public float lifeTime = 20.0f;
	
	public float ignorePlayerTime = 0.5f;	//ignore player for this long
	private float currentTime;

	
	// Use this for initialization
	void Start () {
		//start movement
		rigidbody.AddForce(dirVec * Speed);
		
		currentTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime if collsision is not made
		if(lifeTime + currentTime < Time.time){
			Destroy(gameObject);
		}
	}
	
	
	void OnTriggerEnter(Collider collisions){
		
	//instanite explosion if object is not a player
		if(collisions.gameObject != playerPrefab){
			//instansiate explosion
			Instantiate(ExploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
			//rigidbody.AddForce(5000,0,0);
			
	//instantiate explosion on playerObject if ignorePlayerTime is over
		}else if(ignorePlayerTime + currentTime < Time.time){
			Debug.Log("Player is hit");
			//instansiate explosion
			Instantiate(ExploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
		}
	}
}

