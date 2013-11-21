using UnityEngine;
using System.Collections;

public class Fireball_Projectile : MonoBehaviour {
	
	//directionvektor from player
	public Vector3 dirVec;
	public float Speed = 5.0f;
	public float force = 5.0f;
	
	// Use this for initialization
	void Start () {
	
		//vad ska vi ha för forcemode på denna?
		//deltatime?
		rigidbody.AddForce(dirVec * Speed);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	void OnCollisionEnter(Collision other){
		
		//vi måste sätta eller bestämma hur stor forceVec behöver vara
		//deltatime?
		other.rigidbody.AddForce(dirVec * force, ForceMode.VelocityChange);
		
		//destroy projectile
		Destroy(gameObject);
		
	}
	
}
