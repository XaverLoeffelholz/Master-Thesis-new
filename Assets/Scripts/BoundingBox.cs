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

        GameObject lines = Instantiate(linesPrefab);
        lines.transform.SetParent(transform.GetChild(0));
        lines.GetComponent<Lines>().DrawLinesWorldCoordinate(coordinates);
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
