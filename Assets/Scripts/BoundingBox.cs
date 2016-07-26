using UnityEngine;
using System.Collections;

public class BoundingBox : MonoBehaviour {

	[HideInInspector]
	public Vector3[] coordinates;

	public LineRenderer Top;
	public LineRenderer Bottom;
	public LineRenderer Side1;
	public LineRenderer Side2;
	public LineRenderer Side3;
	public LineRenderer Side4;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowBoundingBox (){
		Top.gameObject.SetActive (true);
		Bottom.gameObject.SetActive (true);
		Side1.gameObject.SetActive (true);
		Side2.gameObject.SetActive (true);
		Side3.gameObject.SetActive (true);
		Side4.gameObject.SetActive (true);
	}

	public void HideBoundingBox (){
		Top.gameObject.SetActive (false);
		Bottom.gameObject.SetActive (false);
		Side1.gameObject.SetActive (false);
		Side2.gameObject.SetActive (false);
		Side3.gameObject.SetActive (false);
		Side4.gameObject.SetActive (false);
	}

	public void DrawBoundingBox(){

		ShowBoundingBox ();

		Top.SetPosition (0, coordinates [0]);
		Top.SetPosition (1, coordinates [1]);
		Top.SetPosition (2, coordinates [2]);
		Top.SetPosition (3, coordinates [3]);
		Top.SetPosition (4, coordinates [0]);

		Bottom.SetPosition (0, coordinates [4]);
		Bottom.SetPosition (1, coordinates [5]);
		Bottom.SetPosition (2, coordinates [6]);
		Bottom.SetPosition (3, coordinates [7]);
		Bottom.SetPosition (4, coordinates [4]);

		Side1.SetPosition (0, coordinates [0]);
		Side1.SetPosition (1, coordinates [4]);

		Side2.SetPosition (0, coordinates [1]);
		Side2.SetPosition (1, coordinates [5]);

		Side3.SetPosition (0, coordinates [2]);
		Side3.SetPosition (1, coordinates [6]);

		Side4.SetPosition (0, coordinates [3]);
		Side4.SetPosition (1, coordinates [7]);
	}
}
