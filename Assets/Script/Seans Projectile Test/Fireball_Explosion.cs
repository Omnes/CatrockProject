using UnityEngine;
using System.Collections;

public class Fireball_Explosion : MonoBehaviour {
	
	//exp
	public float exploIncrSpeed = 1.0f;
	public float exploLifeTime = 0.2f;
	public float force = 5.0f;
	//the direction that other objects will travel if hit
	private Vector3 dirVec;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Destroy(this.gameObject, exploLifeTime);
	}
	
	void OnCollisionEnter(Collision collisions){
		//itterates over all collisionspoints for the different collisions
		
		if(collisions.rigidbody){
			foreach(ContactPoint collision in collisions){
				//räkna ut directionvektorn
				dirVec = Vector3.Normalize(collision.point - (collider as SphereCollider).center);
				collision.otherCollider.rigidbody.AddForce(dirVec * force, ForceMode.Impulse);
				//if object is an enityt, collisions.sendmessage(DoDamge(hp));
			}
		}
	}
	
}
