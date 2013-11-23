using UnityEngine;
using System.Collections;

//abstract base class for spells, use as a public variable
public abstract class Spell : MonoBehaviour {
	
	public abstract void cast(GameObject g);
}
