using UnityEngine;
using System.Collections;
using System;

public class Face : MonoBehaviour {

	public enum faceType {
		TopFace = 0,
		BottomFace = 1,
		SideFace = 2
	}

	public faceType typeOfFace;

    public VertexBundle[] vertexBundles;
    public VertexBundle center;
    public Vector3 centerPosition;
	public Vector3 normal;
    public handle scaleHandle;
    public handle centerHandle;
    public handle heightHandle;

	public GameObject VertexBundlePrefab;
	public GameObject VertexPrefab;

    // Use this for initialization
    void Start () {
		
    }
	
	// Update is called once per frame
	void Update () {
	    if (center != null && centerPosition != center.coordinates)
        {
            UpdateFaceFromCenter();
        }
	}

	public void InitializeFace(int numberOfVertices) {
		vertexBundles = new VertexBundle[numberOfVertices];
	}

    public void scaleFace(float amount)
    {
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != null)
            {
                Vector3 VertexToCenter = vertexBundles[i].coordinates - centerPosition;
                vertexBundles[i].coordinates = vertexBundles[i].coordinates + VertexToCenter * amount;

                vertexBundles[i].coordinates = RasterManager.Instance.Raster(vertexBundles[i].coordinates);
            }

        }
    }

    public void UpdateFaceFromCenter()
    {
        Vector3 difference = center.coordinates - centerPosition;
        centerPosition = center.coordinates;

        // go through each vertex and apply a similar translation
        for (int i=0; i < vertexBundles.Length; i++)
        {
            if (vertexBundles[i] != center)
            {
                vertexBundles[i].coordinates += difference;
            }
        }

        scaleHandle.transform.localPosition = centerPosition;
        centerHandle.transform.localPosition = centerPosition;
        if (heightHandle!= null)
        {
            heightHandle.transform.localPosition = centerPosition;
        }

    }

	public void AddVertexBundle(VertexBundle vBundle){
		for (int i=0; i< vertexBundles.Length; i++){
			if (vertexBundles [i] == null) {
				vertexBundles [i] = vBundle;
				break;
			}
		}

	}

	public void CalculateCenter(){
		
		Vector3 centerPositionNew = new Vector3 (0, 0, 0);

		for (int i = 0; i < vertexBundles.Length; i++) {
			centerPositionNew += vertexBundles [i].coordinates;
		}

		centerPositionNew = centerPositionNew / vertexBundles.Length;

		// check if there is a bundle with the center already
		foreach (Transform child in transform)
		{
			if (child.gameObject.CompareTag("VertexBundle")){
				if (child.gameObject.GetComponent<VertexBundle>().coordinates == centerPositionNew) {
					center = child.gameObject.GetComponent<VertexBundle>();
				}
			}
		}

		if (center == null) {
			GameObject centerGameObject;
			GameObject centerVertexGameObject;

			centerGameObject = Instantiate (VertexBundlePrefab);
			centerVertexGameObject = Instantiate (VertexPrefab);
			centerVertexGameObject.transform.localPosition = centerPositionNew;
			centerVertexGameObject.transform.SetParent (centerGameObject.transform);
			centerGameObject.transform.SetParent (this.transform);
			center = centerGameObject.GetComponent<VertexBundle> ();
			center.coordinates = centerPositionNew;
		}

		centerPosition = centerPositionNew;

		if (typeOfFace == faceType.TopFace) {
			center.name = "Center Top";
			OrderVertexBundlesClockwise ();
		} else if (typeOfFace == faceType.BottomFace) {
			center.name = "Center Bottom";
			OrderVertexBundlesClockwise ();
		} else {
			center.name = "Center Side";
		}

		center.Initialize ();
	}

	public void OrderVertexBundlesClockwise(){
		
		VertexBundle[] OrderedVertexBundles = new VertexBundle[vertexBundles.Length];
		VertexBundle[] LeftVertexBundles = vertexBundles;

		OrderedVertexBundles [0] = vertexBundles [0];

		for (int i = 1; i < OrderedVertexBundles.Length; i++) {

			float distance = 0f;
			int idOfnextBundle = 0;

			// check which vertex bundle is the closes clockwise of the current one
			for (int j = 1; j < LeftVertexBundles.Length; j++) {
				if (LeftVertexBundles[j] != null) {
					float newDistance = Vector3.Dot (normal, Vector3.Cross (OrderedVertexBundles [i-1].coordinates - centerPosition, LeftVertexBundles [j].coordinates - centerPosition));
					if (newDistance > distance) {
						distance = newDistance;
						idOfnextBundle = j;

					}
				}

			}

			distance = 0f;
			OrderedVertexBundles [i] = LeftVertexBundles [idOfnextBundle];
			//Debug.Log ("next point: " + OrderedVertexBundles [i].coordinates);
			LeftVertexBundles [idOfnextBundle] = null;

		}

		vertexBundles = OrderedVertexBundles;

	}

	public void UpdateCenter(){
		Vector3 centerPositionNew = new Vector3 (0, 0, 0);

		for (int i = 0; i < vertexBundles.Length; i++) {
			centerPositionNew += vertexBundles [i].coordinates;
		}

		centerPositionNew = centerPositionNew / vertexBundles.Length;
		center.coordinates = centerPositionNew;
		centerPosition = centerPositionNew;
	}

	public void RecalculateNormal(){
		// maybe we can get it from the mesh, otherwise calculate here
		Vector3 dir = Vector3.Cross(vertexBundles[1].coordinates - vertexBundles[0].coordinates, vertexBundles[2].coordinates - vertexBundles[0].coordinates);
		normal = Vector3.Normalize(dir);
	}

	public void SetType(faceType type){
		typeOfFace = type;
		this.gameObject.name = type.ToString ();
	}

	public void ReplaceFaceOnOtherFace(Face other, Vector3 distance, bool reverseOrder){

		if (!reverseOrder) {
			for (int i = 0; i < vertexBundles.Length; i++) {
                if (i == 0)
                {
                    Vector3 newPoint = other.vertexBundles[vertexBundles.Length - 1].transform.TransformPoint(other.vertexBundles[vertexBundles.Length - 1].coordinates);
                    newPoint += other.vertexBundles[vertexBundles.Length - 1].transform.TransformDirection(distance);
                    vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);


                    // old:
                    //  vertexBundles[i].coordinates = other.vertexBundles[vertexBundles.Length - 1].coordinates + distance;
                }
                else
                {
                    Vector3 newPoint = other.vertexBundles[i - 1].transform.TransformPoint(other.vertexBundles[i - 1].coordinates);
                    newPoint += other.vertexBundles[i - 1].transform.TransformDirection(distance);
                    vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);

                    // old:
                    // vertexBundles[i].coordinates = other.vertexBundles[i - 1].coordinates + distance;
                }
			}
		} else {
			for (int i = 0; i < vertexBundles.Length; i++) {
                Vector3 newPoint = other.vertexBundles[vertexBundles.Length - 1 - i].transform.TransformPoint(other.vertexBundles[vertexBundles.Length - 1 - i].coordinates);
                newPoint += other.vertexBundles[vertexBundles.Length - 1 - i].transform.TransformDirection(distance);
                vertexBundles[i].coordinates = vertexBundles[i].transform.InverseTransformPoint(newPoint);
                
                // old:
                // vertexBundles[i].coordinates = other.vertexBundles[vertexBundles.Length - 1 - i].coordinates + distance;
            }
        }

		UpdateCenter ();
		RecalculateNormal ();
	}

    public void CreateNewModelingObject(){
		ObjectCreator.Instance.createNewObjectOnFace (this);
	}


    public void Highlight()
    {
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            vertexBundles[i].Show();
        }
    }

    public void UnHighlight()
    {
        for (int i = 0; i < vertexBundles.Length; i++)
        {
            vertexBundles[i].Hide();
        }
    }

}
