using UnityEngine;
using System.Collections;

public class StartInLobby : MonoBehaviour {

	void Start() {
		if(Application.loadedLevelName != "lobby" && IveAlreadyDoneIt == false) {
			Debug.Log ("loading lobby");
			Application.LoadLevel("lobby");
		}
		IveAlreadyDoneIt = true;
	}
	
	static private bool IveAlreadyDoneIt = false;
}
