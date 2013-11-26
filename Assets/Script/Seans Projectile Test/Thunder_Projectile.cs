using UnityEngine;
using System.Collections;

public class Thunder_Projectile : MonoBehaviour {

	public Vector3 dirVec;					//directionvektor from player
	public GameObject playerPrefab;			//must get creator from creator to not collide with creator
	public float Speed = 800.0f;
	public GameObject ExploPrefab;
	public float lifeTime = 2.5f;
	
	public float ignorePlayerTime = 0.5f;	//ignore player for this long
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
		if(lifeTime + currentTime < Time.time){
			Destroy(gameObject);
		}
		
		//
		sinCounter.x += 0.1f;
		if(sinTime.x > -0.99){
			sinTime.x = Mathf.Sin(sinCounter.x);
			//forward
			rigidbody.AddForce(dirVec * sinTime.x * multi.x);
			//up
		}
		
		sinCounter.y += 0.1f;
		if(sinTime.y > -0.99){
			sinTime.y = Mathf.Cos(sinCounter.y);
			//forward
			rigidbody.AddForce(perpendicularVector * sinTime.y * multi.y);
			//up
		}
		
	}
	
	
	void OnTriggerEnter(Collider collisions){
		
	/*//instanite explosion if object is not a player
		if(collisions.gameObject != playerPrefab || collisions.gameObject.name != this.gameObject.name){
			//instansiate explosion
			Instantiate(ExploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
			//rigidbody.AddForce(5000,0,0);
			
	//instantiate explosion on playerObject if ignorePlayerTime is over
		}else if(ignorePlayerTime + currentTime < Time.time){
			Debug.Log("Player is hit");
			//instansiate explosion
			Instantiate(ExploPrefab, transform.position, transform.rotation);
			//destroy projectile
			Destroy(gameObject);
		}*/
	}
}




//skjuta spells ?
		/*sinCounter += 0.01f;
		
		if(sinTime > -0.9){
			sinTime = Mathf.Sin(sinCounter);
			rigidbody.AddForce(dirVec * sinTime * multi);
		}*/