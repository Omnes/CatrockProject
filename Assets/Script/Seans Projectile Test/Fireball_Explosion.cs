using UnityEngine;
using System.Collections;

public class Fireball_Explosion : MonoBehaviour {
	
	private Vector3 dirVec;					//the direction that other objects will travel if hit	
	public float exploLifeTime = 0.1f;
	public float exploForce = 5.0f;
	public float exploDamage = 1.0f;

	public GameObject playerPrefab;
	
	public AudioSource exploSound;
	
	bool haveCollided = false;
	
	// Use this for initialization
	void Start () {
		exploSound.Play();
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime is here if collsion is not made
		Destroy(gameObject, exploLifeTime);
	}
	
	void OnCollisionEnter(Collision collisions){
		
		if(collisions.rigidbody && !haveCollided/* && collisions.gameObject.layer != LayerMask.NameToLayer("Projectile")*/){
			//itterates over all collisionspoints for the different collisions
			foreach(ContactPoint collision in collisions){
				//creates directionvector for objects
				dirVec = Vector3.Normalize(collision.point - (collider as SphereCollider).center);
                dirVec *= exploForce;

                collisions.gameObject.SendMessage("TryDoPush", dirVec, SendMessageOptions.DontRequireReceiver);
				//if object is an enityt
				//change this to fit for only entities
				collisions.gameObject.SendMessage("TryDoDamage", exploDamage, SendMessageOptions.DontRequireReceiver);
			}
		}
		haveCollided = true;
		Destroy(gameObject, exploLifeTime);
	}
	
}
