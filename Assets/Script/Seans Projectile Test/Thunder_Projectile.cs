using UnityEngine;
using System.Collections;

public class Thunder_Projectile : MonoBehaviour {

	public Vector3 dirVec;					//directionvektor from player
	public GameObject playerPrefab;			//must get creator from creator to not collide with creator
	public GameObject exploPrefab;
	public float Speed = 5.0f;
	public float lifeTime = 2.5f;
	
	public float ignorePlayerTime = 0.5f;	//ignore player for this long. Will not work in this script
	private float currentTime;
	
	//wave
	private Vector2 sinCounter = new Vector2(-0.8f, 0.0f);
	private Vector2 sinTime;
	public Vector2 multi;
	private Vector3 perpendicularVector;

	
	// Use this for initialization
	void Start () {
		//start movement
		currentTime = Time.time;
		perpendicularVector = Vector3.Cross(dirVec, Vector3.forward.normalized);
	}
	
	// Update is called once per frame
	void Update () {
		//lifetime if collsision is not made
		Destroy(gameObject, lifeTime);
		
		//
		sinCounter.x += 0.1f;
		if(sinTime.x > -0.99){
			sinTime.x = Mathf.Sin(sinCounter.x);
			//forward
			rigidbody.AddForce(dirVec * sinTime.x * multi.x * Speed);
			//up
		}
		
		sinCounter.y += 0.1f;
		if(sinTime.y > -0.99){
			sinTime.y = Mathf.Cos(sinCounter.y);
			//forward
			rigidbody.AddForce(perpendicularVector * sinTime.y * multi.y * Speed);
			//up
		}
		
	}
	
	
	void OnTriggerEnter(Collider collisions){
		
	//instanite explosion if object is not a player
		if(collisions.gameObject != playerPrefab){
			//instansiate explosion
			Instantiate(exploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
	//instantiate explosion on playerObject if ignorePlayerTime is over
		}else if(ignorePlayerTime + currentTime < Time.time){
			Debug.Log("Player is hit");
			//instansiate explosion
			Instantiate(exploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
		}
	}
}