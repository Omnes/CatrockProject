using UnityEngine;
using System.Collections;

public class Melee_Explosion : MonoBehaviour {
	
	private Vector3 dirVec;					//the direction that other objects will travel if hit
	public float exploLifeTime = 0.2f;
	public float exploForce = 5.0f;
	public float exploDamage = 1.0f;
	
	public GameObject player;
	
	// Use this for initialization
	void Start () {
		if(player != null){
			Physics.IgnoreCollision(this.collider, player.collider);
		}else{
			Debug.Log("ERROR: Melee_Explosion, PlayerPrefab not initialized");
		}
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime is here if collsion is not made
		Destroy(gameObject, exploLifeTime);
	}
	
	void OnCollisionEnter(Collision collisions){
		
		if(collisions.gameObject == player){
			Debug.Log("ERROR: Melee_Explosion, Player not supposed to get hit");
		}
		
		if(collisions.gameObject.layer != LayerMask.NameToLayer("Projectile")){
			//itterates over all collisionspoints for the different collisions
			foreach(ContactPoint collision in collisions){
				//creates directionvector for objects
				dirVec = Vector3.Normalize(collision.point - (collider as BoxCollider).center);
				dirVec *= exploForce;
				//collision.otherCollider.rigidbody.AddForce(dirVec * exploForce, ForceMode.VelocityChange);
				
				
				//if object is an enityt
				//change this to fit for only entities
				collisions.gameObject.SendMessage("TryDoPush", dirVec, SendMessageOptions.DontRequireReceiver);
				collisions.gameObject.SendMessage("TryDoDamage", exploDamage, SendMessageOptions.DontRequireReceiver);
			}
		}
		Destroy(gameObject);
	}
	
}
