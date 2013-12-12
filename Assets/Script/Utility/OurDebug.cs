using UnityEngine;
using System.Collections;

public static class OurDebug : object {
	
	public static void Log(string s){
		if(Application.isEditor){
			Debug.Log(s);
		}
	}
}
