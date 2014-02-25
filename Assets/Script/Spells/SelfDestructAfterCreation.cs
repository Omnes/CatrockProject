using UnityEngine;
using System.Collections;

public class SelfDestructAfterCreation : MonoBehaviour {

	public float timer = 5f;
	void Start () {
		Destroy (gameObject,timer);
	}

}
