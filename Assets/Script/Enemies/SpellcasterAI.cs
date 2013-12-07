using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellcasterAI : MonoBehaviour {
	
	public spellBehaviour[] avaliableSpells;
	
	private int counter = 0; //temp
	
	[System.Serializable]
	public class spellBehaviour{
		public Spell spell;
		public float minDistance = 0;
		public float maxDistance = 10;
		public int priority = 0;
		public string name = "";
		public string animation = "";
		public string soundEffect = "";
		
	};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private spellBehaviour chooseSpell(float distance){
		//hitta vilka spells som vi kan använda
		List<spellBehaviour> castableSpells = new List<spellBehaviour>();
		foreach(spellBehaviour sb in avaliableSpells){
			if(distance > sb.minDistance && distance < sb.maxDistance){
				castableSpells.Add(sb);
			}
		}
		if(castableSpells.Count < 1){
			return null;
		}
		
		//hitta den med högst prio
		spellBehaviour highestPrio = castableSpells[0];
		foreach(spellBehaviour s in castableSpells){
			if(s.priority > highestPrio.priority){
				highestPrio = s;
			}
		}
		
		return highestPrio;
		
	}
	
	public void castSpell(GameObject target){
		float dist = Vector3.Distance(transform.position,target.rigidbody.position);
		spellBehaviour spellToCast = chooseSpell(dist);
		
		if(spellToCast == null){
			return;
		}
		
		Debug.Log(gameObject.name + " casts " + spellToCast.name);
		spellToCast.spell.cast(gameObject);
		//spela animationen
		//spela ljudet
		
	}
}
