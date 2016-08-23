using UnityEngine;
using System.Collections;

public class BoundingBox : MonoBehaviour {

	public Vector3[] coordinates;
    public GameObject linesPrefab;
	public GameObject boundingBoxCollider;

	private bool active = false;

	// Use this for initialization
	void Start () {

	}
	
	void FixedUpdate () {
		if (active) {
			boundingBoxCollider.transform.localScale = new Vector3 ((coordinates [0] - coordinates [1]).magnitude, (coordinates [0] - coordinates [4]).magnitude, (coordinates [0] - coordinates [3]).magnitude) / boundingBoxCollider.transform.lossyScale.x;
		}
	}


	public void DrawBoundingBox(){
        ClearBoundingBox();
        GameObject linesGO = Instantiate(linesPrefab);
		linesGO.transform.SetParent(transform.GetChild(0));
		Lines lines = linesGO.GetComponent<Lines> ();

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[0],coordinates[1],coordinates[2],coordinates[3]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[4],coordinates[5],coordinates[6],coordinates[7]});

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[0],coordinates[4]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[1],coordinates[5]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[2],coordinates[6]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinates[3],coordinates[7]});
    }

    public void ClearBoundingBox()
    {
        // delete inside 
        foreach (Transform lineObject in transform.GetChild(0))
        {
            Destroy(lineObject.gameObject);
        }
    }

	public void ActivateBoundingBoxCollider(){
		active = true;
		boundingBoxCollider.GetComponent<BoxCollider>().enabled = true;
	}

	public void DeActivateBoundingBoxCollider(){
		active = false;
		boundingBoxCollider.GetComponent<BoxCollider>().enabled = false;
	}
}
