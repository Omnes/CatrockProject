using UnityEngine;
using System.Collections;

public class Fireball_Projectile : MonoBehaviour {
	
	public Vector3 dirVec;					//directionvektor from player
	public GameObject player;			//must get creator from creator to not collide with creator
	public GameObject exploPrefab;
	public float Speed = 800.0f;
	public float lifeTime = 2.5f;
	
	public float ignorePlayerTime = 0.5f;	//ignore player for this long. Will not work in this script
	private float currentTime;
	public AudioSource fireSound;
	
	// Use this for initialization
	void Start () {
		//start movement
		rigidbody.AddForce(dirVec * Speed);
		currentTime = Time.time;
				
		//test
		fireSound.Play();
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime if collsision is not made
		Destroy(gameObject, lifeTime);
	}
	
	
	void OnTriggerEnter(Collider other){
	
		//ignore specific layermask
		if(other.gameObject.layer != LayerMask.NameToLayer("Explosion")){
		//instanite explosion if object is not a player
			if(other.gameObject != player){
				//instansiate explosion
				Instantiate(exploPrefab, transform.position, transform.rotation);
				Destroy(gameObject);
		//instantiate explosion on playerObject if ignorePlayerTime is over
			}else if(ignorePlayerTime + currentTime < Time.time){
				OurDebug.Log("Player is hit");
				Instantiate(exploPrefab, transform.position, transform.rotation);
				Destroy(gameObject);
			}
		}
	}
}

