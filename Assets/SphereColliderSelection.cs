using UnityEngine;
using System.Collections;

public class SphereColliderSelection : MonoBehaviour {

	public GameObject currentCollider;
	public MeshRenderer renderer;
	public SphereCollider collider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){
		// we can also compare which collider has more points and choose that one
		if (currentCollider == null) {
			currentCollider = collision.collider.gameObject;
		}
	}

	void OnCollisionStay(Collision collision){
		if (currentCollider != null) {
			if (collision.collider.gameObject == null) {
				currentCollider = null;
			}
		}
	}

	void OnCollisionExit(Collision collision){
		if (currentCollider != null && collision.collider.gameObject == currentCollider.gameObject) {
			currentCollider = null;
		}
	}

	public void ActivateCollider(){
		collider.enabled = true;
		renderer.enabled = true;
		currentCollider = null;
	}

	public void DeActivateCollider(){
		collider.enabled = false;
		renderer.enabled = false;
		currentCollider = null;
	}
}
