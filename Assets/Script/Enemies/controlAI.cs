using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class controlAI : MonoBehaviour {

	public enum TargetMode {nearest, random};
	public TargetMode targetMode = TargetMode.nearest;
	public bool ignoreLineOfSight = false;
	public LayerMask losLayerMask;
	public enum AlertState {idle, aggroed};
	public AlertState alertState = AlertState.idle;
	public float aggroDistance = 3;
	public int checkForEnemesInterval = 30;
	public float spellcastCooldown = 2;


	public GameObject currentTarget;

	public GameObject[] playerArray;
	//temp spelldelay
	private float lastCastTime  = 0;
	private int counter = 0;

	// Use this for initialization
	void Awake () {
		playerArray = GameObject.FindGameObjectsWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = transform.position;
		if(alertState == AlertState.idle){
			if(counter % checkForEnemesInterval == 0){
				lookForEnemies();
			}
			//do idle stuff
		}

		if(alertState == AlertState.aggroed){
			//do movement and other stuff here
			if(currentTarget == null){
				alertState = AlertState.idle;
				return;
			}

			//lazy spellcast to see if this works
			if(Time.time > lastCastTime + spellcastCooldown){
				lastCastTime = Time.time;
				SendMessage("castSpell",currentTarget);
			}
		}
	}

	//aggro if a player is close
	void lookForEnemies(){
		Vector3 position = transform.position;
		playerArray = GameObject.FindGameObjectsWithTag("Player");
		List<GameObject> playersInLos = inLoS(playerArray);
		foreach(GameObject p  in playersInLos){
			if(Vector3.Distance(position,p.transform.position) < aggroDistance){
				alertState = AlertState.aggroed;
				chooseTarget();
				break;
			}
		}
	}

	//chose a target from those we can see
	void chooseTarget(){
		playerArray = GameObject.FindGameObjectsWithTag("Player");
		List<GameObject> players = inLoS(playerArray);

		GameObject target = null;
		switch(targetMode){
			case TargetMode.nearest:
				target = targetNearest(players);
			break;
			case TargetMode.random:
				target = targetRandom(players);
			break;
			default:
				Debug.LogError(targetMode + "is not added to the switch in chooseTarget()");
			break;
		}
		if (target != null){
			SendMessage("setTarget",target);
		}

	}

	//return the players the ai can see
	List<GameObject> inLoS(GameObject[] players){
		List<GameObject> playersInLoS = new List<GameObject>();
		if(ignoreLineOfSight){
			return new List<GameObject>(players);
		}
		foreach(GameObject p in players){
			RaycastHit hit;
			if(!Physics.Linecast(transform.position,p.transform.position,out hit,losLayerMask)){
				playersInLoS.Add(p);
			}
		}
		return playersInLoS;
	}
	
	GameObject targetRandom(List<GameObject> players){
		if(players.Count == 0) return null;
		return players[Random.Range(0,players.Count-1)];
	}
	
	GameObject targetNearest(List<GameObject> players){
		if(players.Count == 0) return null;

		GameObject nearest = players[0];
		float dist = Vector3.Distance(transform.position,nearest.transform.position);
		foreach(GameObject p in players){
			float d = Vector3.Distance(transform.position,p.transform.position);
			if(d < dist){
				dist = d;
				nearest = p;
			}
		}
		return nearest;
	}

	void setTarget(GameObject target){
		currentTarget = target;
	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,aggroDistance);
	}


}
