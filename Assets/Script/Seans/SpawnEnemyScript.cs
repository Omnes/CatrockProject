using UnityEngine;
using System.Collections;

public class SpawnEnemyScript : MonoBehaviour {
	
	private float currentTime;
	public float delayTime;
	public GameObject enemyPrefab;
	
	// Use this for initialization
	void Start () {
	
		currentTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
	
		float rndValue = Random.value;
		
		delayTime += rndValue;
		
		if(currentTime + delayTime < Time.time){
			
			Instantiate(enemyPrefab, transform.position, transform.rotation);
			
			
			currentTime = Time.time;
		}
		
		delayTime -= rndValue;
		
	}
}
