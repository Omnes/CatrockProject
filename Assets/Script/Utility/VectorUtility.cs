
// AUTOGENERATED BY JOHANNES MAKEVEC.PY
//
//

using UnityEngine;

namespace Utility {
	
	public static class Vec {


		//GET COMPONENTS FROM VECTORS


		public static Vector4 XYZW(this Vector4 v) {
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public static Vector3 XYZ(this Vector3 v) {
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector3 XYZ(this Vector4 v) {
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector3 XYW(this Vector4 v) {
			return new Vector3(v.x, v.y, v.w);
		}

		public static Vector2 XY(this Vector2 v) {
			return new Vector2(v.x, v.y);
		}

		public static Vector2 XY(this Vector3 v) {
			return new Vector2(v.x, v.y);
		}

		public static Vector2 XY(this Vector4 v) {
			return new Vector2(v.x, v.y);
		}

		public static Vector3 XZW(this Vector4 v) {
			return new Vector3(v.x, v.z, v.w);
		}

		public static Vector2 XZ(this Vector3 v) {
			return new Vector2(v.x, v.z);
		}

		public static Vector2 XZ(this Vector4 v) {
			return new Vector2(v.x, v.z);
		}

		public static Vector2 XW(this Vector4 v) {
			return new Vector2(v.x, v.w);
		}

		public static Vector3 YZW(this Vector4 v) {
			return new Vector3(v.y, v.z, v.w);
		}

		public static Vector2 YZ(this Vector3 v) {
			return new Vector2(v.y, v.z);
		}

		public static Vector2 YZ(this Vector4 v) {
			return new Vector2(v.y, v.z);
		}

		public static Vector2 YW(this Vector4 v) {
			return new Vector2(v.y, v.w);
		}

		public static Vector2 ZW(this Vector4 v) {
			return new Vector2(v.z, v.w);
		}

		//MAKE VECTORS


		public static Vector3 vector3(float x) {
			return new Vector3(x, 0, 0);
		}

		public static Vector3 vector3(float x, float y) {
			return new Vector3(x, y, 0);
		}

		public static Vector3 vector3(Vector2 v) {
			return new Vector3(v.x, v.y, 0);
		}

		public static Vector4 vector4(float x) {
			return new Vector4(x, 0, 0, 0);
		}

		public static Vector4 vector4(float x, float y) {
			return new Vector4(x, y, 0, 0);
		}

		public static Vector4 vector4(Vector2 v) {
			return new Vector4(v.x, v.y, 0, 0);
		}

		public static Vector4 vector4(float x, float y, float z) {
			return new Vector4(x, y, z, 0);
		}

		public static Vector4 vector4(Vector3 v) {
			return new Vector4(v.x, v.y, v.z, 0);
		}

	}
}

