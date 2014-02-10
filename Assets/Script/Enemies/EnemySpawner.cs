using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public Transform EnemyPrefab;

	// Use this for initialization
	void Start () {
		if(Network.isServer){
			Network.Instantiate(EnemyPrefab,transform.position,transform.rotation,0);
		}
	
	}

	[ExecuteInEditMode]
	void OnDrawGizmos(){
		if (EnemyPrefab != null){
			EnemyPrefab.renderer.sharedMaterial.SetPass(0);
			Graphics.DrawMeshNow(EnemyPrefab.GetComponent<MeshFilter>().sharedMesh,Matrix4x4.TRS(transform.position,transform.rotation,transform.localScale));
		}
	}

}
