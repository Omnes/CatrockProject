using UnityEngine;

//Utility functions for floats
public static class FloatUtility {

	private static float EPSILON = 0.01f;

	//check if the difference between two floats is small enough
	public static bool fEqual(float f1, float f2){
		return f1-EPSILON < f2 && f1 + EPSILON > f2; 
	}

	//get the sign of a float(-1, 0 or 1). if the float is close to zero, its sign is 0
	public static float getSign(float n){
		return fEqual(n, 0f) ? 0 : Mathf.Sign(n);
	}

	//from 0..360 to -180..180
	public static float normalizeEulerAngles(float angles) {
		return angles > 179 ? angles - 360 : angles;
	}
}

