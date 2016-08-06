using UnityEngine;
using System.Collections;

public class BoundingBox : MonoBehaviour {

	[HideInInspector]
	public Vector3[] coordinates;

    public GameObject linesPrefab;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
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
}
