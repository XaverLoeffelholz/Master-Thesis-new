using UnityEngine;
using System.Collections;
using System;

public class ModelingObject : MonoBehaviour
{
    public int ObjectID;

    public enum ObjectType
    {
        triangle = 3,
        square = 4,
        hexagon = 6,
        octagon = 8
    }

    public ObjectType typeOfObject;

    public handles handles;
    [HideInInspector]
    public bool selected;

    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] MeshCordinatesVertices;
    private Vector2[] MeshUV;
    private int[] MeshTriangles;

	public Vertex[] vertices;
	public Face[] faces;
    public Face topFace;
    public Face bottomFace;
    public Color color;

    public GameObject VertexPrefab;
    public GameObject VertexBundlePrefab;

	public GameObject NormalPrefab;
	public GameObject Vertex2Prefab;
	public GameObject FacePrefab;

    private Selection controllerForMovement;
    private Vector3 lastPositionController;

    private bool moving = false;
    bool focused = false;

    private Vector3 PositionOnMovementStart;

    public GameObject DistanceVisualPrefab;
    public GameObject CenterVisualPrefab;
    public GameObject GroundVisualPrefab;
    private GameObject GroundVisualOnStartMoving;
    private Transform DistanceVisualisation;

    private Vector3 lastPositionX;
    private Vector3 lastPositionY;
    private bool firstTimeMoving = true;

    private bool snapped = false;
    private Vector3 initialDistancceCenterBottomScaler;

    public VertexBundle scalerObject;

    // Use this for initialization
    void Start()
    {
        handles.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        DistanceVisualisation = ObjectsManager.Instance.DistanceVisualisation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving && !BiManualOperations.Instance.IsScalingStarted())
        {
            // destroz previous distance vis
            foreach (Transform visualObject in DistanceVisualisation)
            {
                if (visualObject.gameObject != GroundVisualOnStartMoving)
                {
                    Destroy(visualObject.gameObject);
                }
            }

            if (!transform.parent.CompareTag("Objects"))
            {
                transform.SetParent(ObjectsManager.Instance.transform);
                library.Instance.ClearLibrary();
                transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }

            Vector3 newPositionCollider = transform.TransformPoint(RasterManager.Instance.Raster(transform.InverseTransformPoint(controllerForMovement.pointOfCollisionGO.transform.position)));

            Vector3 newPositionWorld = this.transform.position + (newPositionCollider - lastPositionController);
            this.transform.position = newPositionWorld;
            this.transform.localPosition = RasterManager.Instance.Raster(this.transform.localPosition);

            // here check for possible snappings
            if (bottomFace.center.possibleSnappingVertexBundle != null)
            {
                if (!snapped || snapped && (newPositionCollider - lastPositionController).sqrMagnitude < 0.05f * transform.lossyScale.x){
                    snapped = true;
                    Vector3 distanceCurrentBottomSnap = bottomFace.center.possibleSnappingVertexBundle.transform.GetChild(0).position - bottomFace.center.transform.GetChild(0).position;
                    this.transform.position = this.transform.position + distanceCurrentBottomSnap;
                }


            } else if (topFace.center.possibleSnappingVertexBundle != null)
            {
                if (!snapped || snapped && (newPositionCollider - lastPositionController).sqrMagnitude < 0.05f * transform.lossyScale.x)
                {
                    snapped = true;
                    Vector3 distanceCurrentTopSnap = topFace.center.possibleSnappingVertexBundle.transform.GetChild(0).position - topFace.center.transform.GetChild(0).position;
                    this.transform.position = this.transform.position + distanceCurrentTopSnap;
                }
            } else 
            {
                snapped = false;
                lastPositionController = newPositionCollider;
            }



            lastPositionX = PositionOnMovementStart;
            lastPositionY = PositionOnMovementStart;

            if (!firstTimeMoving)
            {
                // show amount of movement on x
                if (bottomFace.center.coordinates.x != transform.InverseTransformPoint(PositionOnMovementStart).x)
                {
                    // maybe check local positon
                    int count = RasterManager.Instance.getNumberOfGridUnits(bottomFace.center.coordinates.x, transform.InverseTransformPoint(PositionOnMovementStart).x);

                    for (int i = 0; i <= Mathf.Abs(count); i++)
                    {
                        if (i == 0)
                        {
                            // Display center of object before moving
                            GameObject CenterVisual = Instantiate(CenterVisualPrefab);
                            CenterVisual.transform.SetParent(DistanceVisualisation);
                            CenterVisual.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                            CenterVisual.transform.localScale = new Vector3(1f, 1f, 1f);
                            CenterVisual.transform.position = PositionOnMovementStart;

                            // Display center of object after moving
                            GameObject CenterVisual2 = Instantiate(CenterVisualPrefab);
                            CenterVisual2.transform.SetParent(DistanceVisualisation);
                            CenterVisual2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                            CenterVisual2.transform.localScale = new Vector3(1f, 1f, 1f);
                            CenterVisual2.transform.position = bottomFace.center.transform.GetChild(0).position;

                            // Display outline of groundface
                            GameObject GroundVisual = Instantiate(GroundVisualPrefab);
                            GroundVisual.transform.SetParent(DistanceVisualisation);
                            LineRenderer lines = GroundVisual.GetComponent<LineRenderer>();
                            lines.SetVertexCount(bottomFace.vertexBundles.Length+1);
         
                            for (int j = 0; j <= bottomFace.vertexBundles.Length; j++)
                            {
                                if (j == bottomFace.vertexBundles.Length)
                                {
                                    Vector3 pos = bottomFace.vertexBundles[0].transform.GetChild(0).position;
                                    lines.SetPosition(j, pos);
                                } else
                                {
                                    Vector3 pos = bottomFace.vertexBundles[j].transform.GetChild(0).position;
                                    lines.SetPosition(j, pos);
                                }

                            }

                        }

                        GameObject DistanceVisualX = Instantiate(DistanceVisualPrefab);
                        DistanceVisualX.transform.SetParent(DistanceVisualisation);
                        DistanceVisualX.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        DistanceVisualX.transform.localScale = new Vector3(1f, 1f, 1f);
                        DistanceVisualX.transform.position = PositionOnMovementStart;

                        if (bottomFace.center.coordinates.x > transform.InverseTransformPoint(PositionOnMovementStart).x)
                        {
                            DistanceVisualX.transform.localPosition += new Vector3(i * RasterManager.Instance.rasterLevel, 0f, 0f);
                        }
                        else
                        {
                            DistanceVisualX.transform.localPosition += new Vector3(i * RasterManager.Instance.rasterLevel * (-1.0f), 0f, 0f);
                        }

                        lastPositionX = DistanceVisualX.transform.position;
                        lastPositionY = DistanceVisualX.transform.position;
                    }

                }



                // show amount of movement on y
                if (bottomFace.center.coordinates.y != transform.InverseTransformPoint(PositionOnMovementStart).y)
                {
                    // use raster manager
                    int count = RasterManager.Instance.getNumberOfGridUnits(bottomFace.center.coordinates.y, transform.InverseTransformPoint(PositionOnMovementStart).y);

                    for (int i = 0; i <= Mathf.Abs(count); i++)
                    {
                        GameObject DistanceVisualY = Instantiate(DistanceVisualPrefab);
                        DistanceVisualY.transform.SetParent(DistanceVisualisation);
                        DistanceVisualY.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        DistanceVisualY.transform.localScale = new Vector3(1f, 1f, 1f);
                        DistanceVisualY.transform.position = lastPositionX;

                        if (bottomFace.center.coordinates.y > transform.InverseTransformPoint(PositionOnMovementStart).y)
                        {
                            DistanceVisualY.transform.localPosition += new Vector3(0f, i * RasterManager.Instance.rasterLevel, 0f);
                        }
                        else
                        {
                            DistanceVisualY.transform.localPosition += new Vector3(0f, i * RasterManager.Instance.rasterLevel * (-1.0f), 0f);
                        }

                        lastPositionY = DistanceVisualY.transform.position;
                    }

                }


                // show amount of movement on z
                if (bottomFace.center.coordinates.z != transform.InverseTransformPoint(PositionOnMovementStart).z)
                {
                    // use raster manager
                    int count = RasterManager.Instance.getNumberOfGridUnits(bottomFace.center.coordinates.z, transform.InverseTransformPoint(PositionOnMovementStart).z);

                    for (int i = 0; i <= Mathf.Abs(count); i++)
                    {
                        GameObject DistanceVisualZ = Instantiate(DistanceVisualPrefab);
                        DistanceVisualZ.transform.SetParent(DistanceVisualisation);
                        DistanceVisualZ.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        DistanceVisualZ.transform.localScale = new Vector3(1f, 1f, 1f);
                        DistanceVisualZ.transform.GetChild(0).localEulerAngles = new Vector3(0f, 90f, 0f);
                        DistanceVisualZ.transform.position = lastPositionY;

                        if (bottomFace.center.coordinates.z > transform.InverseTransformPoint(PositionOnMovementStart).z)
                        {
                            DistanceVisualZ.transform.localPosition += new Vector3(0f, 0f, i * RasterManager.Instance.rasterLevel);
                        }
                        else
                        {
                            DistanceVisualZ.transform.localPosition += new Vector3(0f, 0f, i * RasterManager.Instance.rasterLevel * (-1.0f));
                        }
                    }

                }


            }

    
        }
    }



    public void Initiate(Mesh initialShape)
    {
        this.transform.GetChild(0).GetComponent<MeshFilter>().mesh = initialShape;

        mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        meshCollider = this.transform.GetChild(0).GetComponent<MeshCollider>();
        meshCollider.sharedMesh = initialShape;

        MeshCordinatesVertices = mesh.vertices;
        MeshTriangles = mesh.triangles;
		mesh.RecalculateNormals ();

		int normalCount = 0;
		int vertexCount = 0;

        MeshUV = mesh.uv;

		vertices = new Vertex[MeshCordinatesVertices.Length];


		for (int i=0; i < MeshCordinatesVertices.Length; i++)
        {
            GameObject vert = Instantiate(VertexPrefab);

            vert.transform.localPosition = MeshCordinatesVertices[i];
            vertices[i] = vert.GetComponent<Vertex>();

			// the normals in the mesh have the same Ids as the vertices, so we can safe a normal for every vertex
			vertices [i].normal = mesh.normals [i];
        }

		GetFacesBasedOnNormals ();

		for (int i=0; i < MeshCordinatesVertices.Length; i++)
		{
			if (MeshCordinatesVertices [i].y > 0.0f) {
				vertices[i].transform.SetParent (topFace.gameObject.transform);
			} else {
				vertices[i].transform.SetParent (bottomFace.gameObject.transform);
			}

			vertices [i].Initialize ();
		}

		mesh.vertices = MeshCordinatesVertices;

        BundleSimilarVertices(topFace.gameObject.transform);
        BundleSimilarVertices(bottomFace.gameObject.transform);

		AssignVertexBundlesToFaces ();

		for (int j = 0; j < faces.Length; j++) {
			faces[j].CalculateCenter();
		}
			
		InitializeVertices ();

		// always group 4 vertexbundles to new faces

		/*

		while(vertexCount<mesh.triangles.Length && normalCount < mesh.normals.Length) {
			// go through all triangles (for example 12 in cube)

			// check the normal, if there is already a triangle with this normal, don't do anything

			// otherwise create new normal and take the normal of this triangle as the normal of this face

			// compare it to the normals of other triangles, if similar ad its vertices to a face

			// calculate center when we have all vertices

			// when we have all faces we need to check for top and bottom face
GetFaceFromCollisionCoordinate
			// if newly created one, just by y-value

			// if it is created on top of another, put the duplicated one in the bottom group and the others in the top group



			Vector3 point1 = MeshCordinatesVertices[mesh.triangles [vertexCount]];
			Vector3 point2 = MeshCordinatesVertices[mesh.triangles [vertexCount+1]];
			Vector3 point3 = MeshCordinatesVertices[mesh.triangles [vertexCount+2]];

			Vector3 center = (point1 + point2 + point3) / 3;

			GameObject normal = Instantiate (NormalPrefab);
			normal.transform.SetParent (transform.GetChild(0));
			normal.transform.position = transform.position + mesh.normals[normalCount]*1.2f;

			GameObject normal2 = Instantiate (NormalPrefab);
			normal2.transform.SetParent (transform.GetChild(0));
			normal2.transform.position = transform.position + mesh.normals[normalCount]*1.8f;

			GameObject p1 = Instantiate (Vertex2Prefab);
			p1.transform.SetParent (transform.GetChild(0));
			p1.transform.position = point1  + mesh.normals[normalCount]*0.08f;

			GameObject p2 = Instantiate (Vertex2Prefab);
			p2.transform.SetParent (transform.GetChild(0));
			p2.transform.position = point2 + mesh.normals[normalCount]*0.08f;

			GameObject p3 = Instantiate (Vertex2Prefab);
			p3.transform.SetParent (transform.GetChild(0));
			p3.transform.position = point3 + mesh.normals[normalCount]*0.08f;



			Color randomColor = new Color (Random.value, Random.value, Random.value, 1f);
			normal.GetComponent<Renderer> ().material.color = randomColor;
			normal2.GetComponent<Renderer> ().material.color = randomColor;


			p1.GetComponent<Renderer> ().material.color = randomColor;
			p2.GetComponent<Renderer> ().material.color = randomColor;
			p3.GetComponent<Renderer> ().material.color = randomColor;

			normalCount += 2;
			vertexCount += 3;
		}

		*/

        }

    public void RecalculateNormals()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].RecalculateNormal();
        }
    }

    public void RecalculateCenters()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
        }
    }
    public void RecalculateSideCenters()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i].typeOfFace == Face.faceType.SideFace)
            {
                faces[i].UpdateCenter();
            }
        }
    }


    public void InitializeVertices () {
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i].Initialize ();
		}
	}

	public void ShowNormals(){
		for (int i = 0; i < faces.Length; i++) {
			Debug.DrawLine(faces[i].center.transform.position, faces[i].center.transform.position + faces[i].normal*3.0f, Color.red, 500f);
		}
	}

    public void BundleSimilarVertices(Transform face)
    {

        Transform[] allChildren = face.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allChildren.Length; i++)
        {
            Vector3 position = allChildren[i].localPosition;

            // get all Vertex bundles
            foreach (Transform vertexBundle in face)
                if (vertexBundle.CompareTag("VertexBundle"))
                {

                    // compare position of Vertex with position of vertex bundle, if similar, set vertex bundle as parent
                    if (position == vertexBundle.GetComponent<VertexBundle>().coordinates)
                    {
                        allChildren[i].SetParent(vertexBundle);
                    }

                }

            // if no similar found, create new vertex bundle
			if (!allChildren[i].parent.gameObject.CompareTag("VertexBundle"))
            {
                GameObject vertexBundle = Instantiate(VertexBundlePrefab);
                vertexBundle.transform.SetParent(face);
                vertexBundle.transform.localPosition = new Vector3(0, 0, 0);

                vertexBundle.GetComponent<VertexBundle>().coordinates = allChildren[i].transform.localPosition;

                allChildren[i].SetParent(vertexBundle.transform);

				if (vertexBundle.transform.childCount == 0) {
					Destroy (vertexBundle);
				} else {
					face.GetComponent<Face> ().AddVertexBundle (vertexBundle.GetComponent<VertexBundle>());
				}
	         }
        }
    }

    public void UpdateMesh()
    {
        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            MeshCordinatesVertices[i] = vertices[i].transform.localPosition;
        }

        mesh.Clear();
        mesh.vertices = MeshCordinatesVertices;
        mesh.uv = MeshUV;
        mesh.triangles = MeshTriangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

    public void InitiateHandles()
    {
        handles.faceTopScale.transform.localPosition = topFace.centerPosition;
        handles.faceBottomScale.transform.localPosition = bottomFace.centerPosition;
        handles.CenterTopPosition.transform.localPosition = topFace.centerPosition;
        handles.CenterBottomPosition.transform.localPosition = bottomFace.centerPosition;
        handles.HeightTop.transform.localPosition = topFace.centerPosition;
        handles.HeightBottom.transform.localPosition = bottomFace.centerPosition;

        handles.RotationX.transform.localPosition = this.transform.localPosition;
        handles.RotationY.transform.localPosition = this.transform.localPosition;
        handles.RotationZ.transform.localPosition = this.transform.localPosition;
       
        handles.faceTopScale.transform.localRotation = Quaternion.FromToRotation(handles.faceTopScale.transform.up, topFace.normal);
        handles.CenterTopPosition.transform.localRotation = Quaternion.FromToRotation(handles.CenterTopPosition.transform.up, topFace.normal);
        handles.HeightTop.transform.localRotation = Quaternion.FromToRotation(handles.HeightTop.transform.up, topFace.normal);

        handles.faceBottomScale.transform.localRotation = Quaternion.FromToRotation(handles.faceBottomScale.transform.up, bottomFace.normal);
        handles.CenterBottomPosition.transform.localRotation = Quaternion.FromToRotation(handles.CenterBottomPosition.transform.up, bottomFace.normal);
        handles.HeightBottom.transform.localRotation = Quaternion.FromToRotation(handles.HeightBottom.transform.up, bottomFace.normal);

        handles.faceTopScale.GetComponent<handle> ().face = topFace;
		handles.faceBottomScale.GetComponent<handle> ().face = bottomFace;

		handles.CenterTopPosition.GetComponent<handle> ().face = topFace;
		handles.CenterBottomPosition.GetComponent<handle> ().face = bottomFace;

		handles.HeightTop.GetComponent<handle> ().face = topFace;
        handles.HeightBottom.GetComponent<handle>().face = bottomFace;

        topFace.centerHandle = handles.CenterTopPosition.GetComponent<handle> ();
		topFace.heightHandle = handles.HeightTop.GetComponent<handle> ();
		topFace.scaleHandle = handles.faceTopScale.GetComponent<handle> ();

		bottomFace.centerHandle = handles.CenterBottomPosition.GetComponent<handle> ();
		bottomFace.heightHandle = handles.HeightBottom.GetComponent<handle> ();
		bottomFace.scaleHandle = handles.faceBottomScale.GetComponent<handle> ();

    }

    public void Focus(Selection controller)
    {
		if (!focused) {
            controller.AssignCurrentFocus(transform.gameObject);
			focused = true;
		}
    }

    public void UnFocus(Selection controller)
    {
		if (focused) {
            controller.DeAssignCurrentFocus(transform.gameObject);
			focused = false;
		}
    }

    public void Select(Selection controller)
    {
        controller.AssignCurrentSelection(transform.gameObject);
        handles.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        Vector3 uiPosition = transform.position;
        UiCanvasGroup.Instance.transform.position = uiPosition;
        UiCanvasGroup.Instance.OpenMainMenu(this);

    }

    public void DeSelect(Selection controller)
    {
        controller.DeAssignCurrentSelection(transform.gameObject);
        handles.DisableHandles();
    }

	public void AssignVertexBundlesToFaces(){
		
		// go through all vertices
		for (int i = 0; i < vertices.Length; i++) {

			// check normal of vertice and compare with normal of face
			for (int j = 0; j < faces.Length; j++) {

				// if normals are similar, linke parent vertex bundle to face
				if (vertices [i].normal == faces [j].normal) {
                    if (faces[j].typeOfFace == Face.faceType.SideFace)
                    {
                        faces[j].AddVertexBundle(vertices[i].transform.GetComponentInParent<VertexBundle>());
                    }
				} 
			}
		}
	}


    public void StartMoving (Selection controller)
    {
        moving = true;
        controllerForMovement = controller;
        lastPositionController = controller.pointOfCollisionGO.transform.position;
        PositionOnMovementStart = transform.TransformPoint(bottomFace.center.coordinates);

        // Display outline of groundface
        GroundVisualOnStartMoving = Instantiate(GroundVisualPrefab);
        GroundVisualOnStartMoving.transform.SetParent(DistanceVisualisation);
        LineRenderer lines = GroundVisualOnStartMoving.GetComponent<LineRenderer>();
        lines.SetVertexCount(bottomFace.vertexBundles.Length + 1);

        for (int j = 0; j <= bottomFace.vertexBundles.Length; j++)
        {
            if (j == bottomFace.vertexBundles.Length)
            {
                Vector3 pos = bottomFace.vertexBundles[0].transform.GetChild(0).position;
                lines.SetPosition(j, pos);
            }
            else
            {
                Vector3 pos = bottomFace.vertexBundles[j].transform.GetChild(0).position;
                lines.SetPosition(j, pos);
            }

        }

    }

    public void StopMoving(Selection controller)
    {
        firstTimeMoving = false;
        moving = false;
        controllerForMovement = null;

        // destroz previous distance vis
        foreach (Transform visualObject in DistanceVisualisation)
        {
            Destroy(visualObject.gameObject);
        }
    }

    

    public void GetFacesBasedOnNormals(){

		//currently we have top and bottom face 2 times

		int arrayLength = 0;

		switch (typeOfObject) {
			case ObjectType.triangle:
				arrayLength = 5;
				break;
			case ObjectType.square:
				arrayLength = 6;
				break;
			case ObjectType.hexagon:
				arrayLength = 8;
				break;
			case ObjectType.octagon:
				arrayLength = 10;
				break;
		}

		faces = new Face[arrayLength];

		int faceCount = 0;
		Face faceFound;

		// go trough all vertices
		for (int i = 0; i < vertices.Length; i++) {
			faceFound = null;

			// check for each vertex every face and 
			for (int j = 0; j < faces.Length; j++) {
				
				if (faces [j] != null && vertices [i].normal == faces [j].normal) {
					faceFound = faces [j];
				} 

			}

			if (faceFound == null) {
				GameObject newFace = Instantiate (FacePrefab);
				newFace.transform.SetParent (transform.GetChild(0));
				faces [faceCount] = newFace.GetComponent<Face> ();

				// Check if it is top face? Or not create new face if it is the top/bottom face
				if (vertices [i].normal.x == 0 && vertices [i].normal.z == 0) {

					switch (typeOfObject) {
					case ObjectType.triangle:
						faces [faceCount].InitializeFace (3);
						break;
					case ObjectType.square:
						faces [faceCount].InitializeFace (4);
						break;
					case ObjectType.hexagon:
						faces [faceCount].InitializeFace (6);
						break;
					case ObjectType.octagon:
						faces [faceCount].InitializeFace (8);
						break;
					}
						
					//if direction is up, it is the top face (number of vertices depends on type)
					if (vertices [i].normal.y > 0) {
						faces [faceCount].SetType (Face.faceType.TopFace);
						topFace = newFace.GetComponent<Face>();

					//if direction is down, it is the bottom face (number of vertices depends on type)
					} else {
						faces [faceCount].SetType (Face.faceType.BottomFace);
						bottomFace = newFace.GetComponent<Face>();
					}

				// others are side faces (4 vertices)
				}  else {
					faces [faceCount].InitializeFace (4);
					faces [faceCount].SetType (Face.faceType.SideFace);
				}

				faces [faceCount].normal = vertices [i].normal;

                faceCount++;
			}
		}
	}

    public void CorrectOffset()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].transform.localPosition = new Vector3(0f, transform.localPosition.y * (-1f), 0f);
        }
    }

	public Face GetFaceFromCollisionCoordinate(Vector3 pointOfCollision) {

        // use dot product to check if point lies on a face
        int idOfSmallest = -1;

		for (int i = 0; i < faces.Length; i++) {

			if (Mathf.Abs(Vector3.Dot((faces[i].center.transform.GetChild(0).transform.position - pointOfCollision), faces[i].normal)) <= 0.1){
                idOfSmallest = i;
			}
		}

        if (idOfSmallest != -1)
        {
            return faces[idOfSmallest];
        } else
        {
            return null;
        }

	}

    public void StartScaling()
    {
        initialDistancceCenterBottomScaler = scalerObject.coordinates - bottomFace.centerPosition;
    }

    public void ScaleBy(float newScale)
    {
        Vector3 positionScalerObject = RasterManager.Instance.Raster(bottomFace.centerPosition + ((1f+newScale) * initialDistancceCenterBottomScaler));
        Vector3 newDistanceCenterBottomScaler = positionScalerObject - bottomFace.centerPosition;

        // Go through all Vertex Bundles

        float amount = (positionScalerObject - bottomFace.centerPosition).magnitude / (scalerObject.coordinates - bottomFace.centerPosition).magnitude;

        // get difference to last state to adjust other vertexbundles accordingly
        scalerObject.coordinates = positionScalerObject;

        bottomFace.ReplaceFacefromObjectScaler(bottomFace.centerPosition, amount);
        topFace.ReplaceFacefromObjectScaler(bottomFace.centerPosition, amount);
 
    }

    public void UpDateObjectFromCorner()
    { 
        
        // Get distance from scaler to  ground
        float lengthScalerToCenterBottomFace = (scalerObject.coordinates - bottomFace.centerPosition).magnitude / initialDistancceCenterBottomScaler.magnitude;

        if (Vector3.Dot((scalerObject.coordinates - bottomFace.centerPosition), (scalerObject.coordinates - bottomFace.centerPosition)) < 0f)
        {
            lengthScalerToCenterBottomFace = (-1f) * lengthScalerToCenterBottomFace;
        }


    }


}
		

 
 