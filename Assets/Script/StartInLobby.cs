using UnityEngine;
using System.Collections;

public class StartInLobby : MonoBehaviour {
	public GameObject mastermindPrefab;

	void Start() {
		if(Application.loadedLevelName != "lobby" && IveAlreadyDoneIt == false) {
			Debug.Log ("loading lobby");
			Application.LoadLevel("lobby");
			
		}
		if(IveAlreadyDoneIt == false){
			Instantiate(mastermindPrefab,new Vector3(0,0,0),Quaternion.identity);
		}
		IveAlreadyDoneIt = true;
	}
	
	static private bool IveAlreadyDoneIt = false;
}
