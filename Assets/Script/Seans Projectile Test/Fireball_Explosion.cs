using UnityEngine;
using System.Collections;

public class Fireball_Explosion : MonoBehaviour {
	
	
	public float exploLifeTime = 0.2f;
	public float exploForce = 5.0f;
	public float exploDamage = 1.0f;
	
	private Vector3 dirVec;					//the direction that other objects will travel if hit
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime is here if collsion is not made
		Destroy(gameObject, exploLifeTime);
	}
	
	void OnCollisionEnter(Collision collisions){
		
		if(collisions.rigidbody){
			//itterates over all collisionspoints for the different collisions
			foreach(ContactPoint collision in collisions){
				//creates directionvector for objects
				dirVec = Vector3.Normalize(collision.point - (collider as SphereCollider).center);
				collision.otherCollider.rigidbody.AddForce(dirVec * exploForce, ForceMode.Impulse);
				
				//if object is an enityt
				//change this to fit for only entities
				//collisions.gameObject.GetComponent<Script>().SendMessage("DoDamage", exploDamage);
			}
		}
		Destroy(gameObject);
	}
	
}
