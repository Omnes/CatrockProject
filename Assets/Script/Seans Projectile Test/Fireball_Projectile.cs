using UnityEngine;
using System.Collections;

public class Fireball_Projectile : MonoBehaviour {
	
	//directionvektor from player
	public Vector3 dirVec;
	public float Speed = 5.0f;
	public float force = 5.0f;
	
	
	//public mesh eller animation här 
	public float exploIncrSpeed = 1.0f;
	public bool isExploding = false;
	public float exploLifeTime = 0.2f;
	
	
	// Use this for initialization
	void Start () {
	
		//vad ska vi ha för forcemode på denna?
		//deltatime?
		rigidbody.AddForce(dirVec * Speed);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(isExploding){
			//increase size gradually
			//could make explosion and then remove rigidbody for only visual effects
			(transform as Transform).localScale += new Vector3(exploIncrSpeed,exploIncrSpeed,exploIncrSpeed);
		}
		
	}
	
	
	void OnCollisionEnter(Collision other){
		//stop velocity when projectile hit object
		rigidbody.velocity = Vector3.zero;
		//enable isExploding, will cause scale to increase (se Update)
		isExploding = true;

		if(other.rigidbody){
			//vi måste sätta eller bestämma hur stor forceVec behöver vara
			//deltatime?
			
			//måste istället ta vectorn från mitten av spheren till 
			//collisionHit för att bestämma directionVec
			other.rigidbody.AddForce(dirVec * force, ForceMode.VelocityChange);
			
		}
		//destroy projectile
		Destroy(gameObject, exploLifeTime);
		//Destroy(gameObject);
	}
}

