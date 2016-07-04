using UnityEngine;
using System.Collections;

public class Vertex : MonoBehaviour {

    public VertexBundle parentVertexBundle;
    public ModelingObject parentObject;
	public Vector3 normal;
    public bool moving;
	public GameObject normalPrefab;
	public bool initialized = false;


    // Use this for initialization
    void Start () {
        moving = false;
    }

	public void Initialize() {

		parentVertexBundle = transform.parent.GetComponent<VertexBundle>();
		parentObject = transform.parent.parent.parent.parent.GetComponent<ModelingObject>();
		initialized = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if (initialized && parentVertexBundle.coordinates != this.transform.localPosition)
        {
            if (moving)
            {
                UpdateVertexBundle();
            } else
            {
                UpdatePositionFromVertexBundle();
            }

            parentObject.UpdateMesh();
        }
	}

    public void UpdatePositionFromVertexBundle()
    {
        this.transform.localPosition = parentVertexBundle.coordinates;
    }

    public void UpdateVertexBundle()
    {
        parentVertexBundle.coordinates = this.transform.localPosition;
    }

	public void ShowNormal()
    {
		if (normal != null) {
			GameObject normalVisualisation = Instantiate (normalPrefab);
			normalVisualisation.transform.SetParent (this.transform);
			normalVisualisation.transform.localPosition = transform.localPosition + normal*1.3f;
		}

	}

    void OnCollisionEnter(Collision col)
    {
        if (parentObject.moving)
        {
            if (parentVertexBundle.centerVertex && col.collider.gameObject.CompareTag("Vertex") && col.collider.transform.parent != transform.parent)
            {
                VertexBundle colliderVertBundle = col.collider.transform.parent.GetComponent<VertexBundle>();

                if (colliderVertBundle.centerVertex)
                {
                    parentVertexBundle.possibleSnappingVertexBundle = colliderVertBundle;
                }
            }
            else if (parentVertexBundle.centerVertex && col.collider.gameObject.CompareTag("Ground"))
            {
                parentVertexBundle.possibleGroundSnapping = col.collider.gameObject;
            }
            else if (parentVertexBundle.centerVertex && col.collider.gameObject.CompareTag("SnappingLine"))
            {
                parentVertexBundle.possibleLineSnapping = col.collider.gameObject;
            }
        }  
    }

    void OnCollisionExit(Collision col)
    {
        if (parentObject.moving)
        {
            if (col.collider.gameObject.CompareTag("Vertex") && col.collider.transform.parent != transform.parent)
            {
                if (col.collider.transform.parent.GetComponent<VertexBundle>().centerVertex)
                {
                    parentVertexBundle.possibleSnappingVertexBundle = null;
                }

            }
            else if (parentVertexBundle.centerVertex && col.collider.gameObject.CompareTag("Ground"))
            {
                parentVertexBundle.possibleGroundSnapping = null;
            }
            else if (parentVertexBundle.centerVertex && col.collider.gameObject.CompareTag("SnappingLine"))
            {
                parentVertexBundle.possibleLineSnapping = null;
            }
        }
    }
}
