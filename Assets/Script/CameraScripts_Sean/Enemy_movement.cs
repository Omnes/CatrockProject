using UnityEngine;
using System.Collections;

public class Enemy_movement : MonoBehaviour {
	
	private float currentTime;
	public float lifeTime;
	private bool isMoving;
	
	// Use this for initialization
	void Start () {
		currentTime = Time.time;
		isMoving = true;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(isMoving){
			transform.position += new Vector3(-0.1f,0,0);
		}
		KillMe();
		
	}
	
	void KillMe(){
		
		if(currentTime + lifeTime < Time.time){
			
			Destroy(this.gameObject);
			
		}
	}
	
	void OnCollisionEnter(Collision other){
		
		if(other.collider.tag == "FriendlyBullet"){
		
			isMoving = false;
			
		}
	}
}
