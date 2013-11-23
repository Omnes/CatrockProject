using UnityEngine;
using System.Collections;

public class Fireball_Explosion : MonoBehaviour {
	
	//exp
	public float exploIncrSpeed = 1.0f;
	public float exploLifeTime = 0.2f;
	public float force = 5.0f;
	//the rate that the explosion increasese with
	private Vector3 exploIncrVec;
	//the direction that other objects will travel if hit
	private Vector3 dirVec;
	
	
	// Use this for initialization
	void Start () {
		exploIncrVec = Vector3.one * exploIncrSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		//increase size gradually
		//could make explosion and then remove rigidbody for only visual effects
		transform.localScale += exploIncrVec;
		
	}
	
	void OnCollisionEnter(Collision collisions){
		//itterates over all collisionspoints for the different collisions
		foreach(ContactPoint collision in collisions){
			//räkna ut directionvektorn
			dirVec = collision.point - transform.position;
			dirVec = dirVec.normalized;
			collision.otherCollider.rigidbody.AddForce(dirVec * force, ForceMode.Impulse);
			//if object is an enityt, collisions.sendmessage(DoDamge(hp));
		}
		//destroy collider
		Destroy (gameObject.collider);
		//Destroy objekt after certain amount of time
		Destroy(gameObject, exploLifeTime);
	}
}
