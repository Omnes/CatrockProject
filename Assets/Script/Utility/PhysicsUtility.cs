
using UnityEngine;

public class PhysicsUtility
{
	static public RaycastHit mouseHit() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
			return hit;
		} else {
			OurDebug.Log("mouse ray didn't hit anything");
			Debug.Break();
			return hit; //silence compiler complaints about not returning a value
		}

	}
}

