using UnityEngine;
using System.Collections;

public class Fireball_Projectile : MonoBehaviour {
	
	//directionvektor from player
	public Vector3 dirVec;
	public float Speed = 5.0f;
	public GameObject ExploPrefab;
	
	// Use this for initialization
	void Start () {
		//start movement
		rigidbody.AddForce(dirVec * Speed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	void OnCollisionEnter(Collision other){
		//instansiate explosion
		Instantiate(ExploPrefab, transform.position, transform.rotation);

		//destroy projectile
		Destroy(gameObject);
	}
}

