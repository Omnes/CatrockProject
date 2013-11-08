using UnityEngine;
using System.Collections;

public class Bullet_movement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Translate(0,0,-0.5f);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		
		KillMe();
		
	}
	
	void KillMe(){
		
		Destroy(gameObject, 1);
		
	}
}
