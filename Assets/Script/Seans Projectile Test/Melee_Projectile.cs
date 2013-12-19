using UnityEngine;
using System.Collections;

public class Melee_Projectile : MonoBehaviour {
	
	public Vector3 dirVec;					//directionvektor from player
	public GameObject player;			//must get creator from creator to not collide with creator
	public GameObject exploPrefab;
	public float Speed = 0.0f;				//this will not be used in this script
	public float lifeTime = 1.0f;
	
	public float ignorePlayerTime = 0.5f;	//ignore player for this long. Will not work in this script
	private float currentTime;
	
	// Use this for initialization
	void Start(){
		transform.position += new Vector3(dirVec.x * (transform.localScale.x/2), transform.position.y, 0);
		
		
		//ignore player
		if(player != null){
			Physics.IgnoreCollision(this.collider, player.collider);
		}else{
			Debug.Log("ERROR: Melee_Projectile, PlayerPrefab not initialized");
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		Destroy(gameObject, lifeTime);
	}
	
	
	void OnTriggerEnter(Collider collisions){
		
		if(collisions.gameObject == player){
			Debug.Log("ERROR: Melee_Projectile, Player not supposed to get hit");
		}
		if(collisions.gameObject.layer != LayerMask.NameToLayer("Explosion") && collisions.rigidbody){
			//instansiate explosion
			GameObject newExplo = (GameObject) Instantiate(exploPrefab, transform.position, transform.rotation);
			Melee_Explosion explo = newExplo.GetComponent<Melee_Explosion>();
			explo.player = player;
			Destroy(gameObject);
		}
	}
}