using UnityEngine;
using System.Collections;

public class Melee_Projectile : MonoBehaviour {

	//directionvektor from player
	public Vector3 dirVec;
	public float Speed = 5.0f;
	public GameObject ExploPrefab;
	
	// Use this for initialization
	void Start () {
		transform.position += new Vector3(dirVec.x * (transform.localScale.x/2), transform.position.y, transform.position.z);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Destroy(gameObject, 1.0f);
	}
	
	
	void OnCollisionEnter(Collision other){
		//instansiate explosion
		//Instantiate(ExploPrefab, other.contacts[0].point, transform.rotation);

		//destroy projectile
		//Destroy(gameObject);
	}
}

