using UnityEngine;
using System.Collections;

public class VertexBundle : MonoBehaviour {

    public Vector3 coordinates;
	public VertexBundle nextBundle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize() {
		// initiate all children that are of type Vertex
		Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

		foreach (Vertex vert in vertices) {
			vert.Initialize ();
		}
	}

    public void Show() {
        Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

        foreach (Vertex vert in vertices)
        {
            vert.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void Hide()
    {
        Vertex[] vertices = transform.GetComponentsInChildren<Vertex>();

        foreach (Vertex vert in vertices)
        {
            vert.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
