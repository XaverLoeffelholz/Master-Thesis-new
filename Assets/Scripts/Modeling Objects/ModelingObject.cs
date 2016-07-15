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
    public bool selected = false;

    public Mesh mesh;
    private MeshCollider meshCollider;
    public Vector3[] MeshCordinatesVertices;
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

    public bool moving = false;
    bool focused = false;

    private Vector3 PositionOnMovementStart;

    public GameObject DistanceVisualPrefab;
    public GameObject CenterVisualPrefab;
    public GameObject GroundVisualPrefab;
    private GameObject GroundVisualOnStartMoving;
	private GameObject GroundVisualOnStartMovingTop;
	private GameObject GroundVisualOnStartMovingBottom;
    private Transform DistanceVisualisation;

    private Vector3 lastPositionX;
    private Vector3 lastPositionY;
    private bool firstTimeMoving = true;

    private bool snapped = false;
    private Vector3 initialDistancceCenterBottomScaler;

    public VertexBundle scalerObject;

    public bool inTrashArea = false;
    public GameObject coordinateSystem;
    public Group group;

    public ObjectSelecter objectSelector;

    private Vector3 relativeTo;
	private Vector3 initialPosition;

	private Transform player;

	public GameObject trashIcon;

	private Color currentColor;

    // Use this for initialization
    void Start()
    {
        handles.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        DistanceVisualisation = ObjectsManager.Instance.DistanceVisualisation;
		player = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving && !BiManualOperations.Instance.IsScalingStarted())
        {
			if (inTrashArea) {
				trashIcon.transform.parent.transform.LookAt (player);
			}

            Vector3 prevPosition = transform.position;

            // destroz previous distance vis
            foreach (Transform visualObject in DistanceVisualisation)
            {
                if (visualObject.gameObject != GroundVisualOnStartMoving)
                {
                    Destroy(visualObject.gameObject);
                }
            }


            // if the user takes an object from the library, delete other objects in the library
            // we could also just create a new version of the taken object

            if (transform.parent.CompareTag("Library"))
            {
				//Vector2 distanceToInitialPosition = new Vector2 (initialPosition.x - transform.position.x, initialPosition.z - transform.position.z);

				//if (distanceToInitialPosition.magnitude > transform.lossyScale.x*5f) {
					transform.SetParent(ObjectsManager.Instance.transform);
					library.Instance.ClearLibrary();                

					LeanTween.scale (this.gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeInOutExpo);
					LeanTween.rotateLocal (this.gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutExpo);
				//}

            }

            Vector3 newPositionCollider = transform.TransformPoint(RasterManager.Instance.Raster(transform.InverseTransformPoint(controllerForMovement.pointOfCollisionGO.transform.position)));
			Vector3 newPositionWorld = this.transform.position + (newPositionCollider - lastPositionController);
			this.transform.position = newPositionWorld;

			Vector3 boundingBoxBottomCenter = transform.TransformPoint(GetBoundingBoxBottomCenter ());


			/*

			// Work on this later, block movement below 0, currently not working

			// check if lowest point of object touches the ground
			if (boundingBoxBottomCenter.y <= ObjectsManager.Instance.stageScaler.transform.position.y) {

				// check how far it is below
				float belowZero = ObjectsManager.Instance.stageScaler.transform.position.y - boundingBoxBottomCenter.y;
				Vector3 bottomPointOnGround = boundingBoxBottomCenter + new Vector3 (0f, belowZero, 0f);
				float distancePointToCollisionGO = Vector3.Distance (newPositionCollider, bottomPointOnGround);

				// get Vector between center bottom and collider
				Vector3 CenterBottomToCollisionGO = (newPositionCollider - boundingBoxBottomCenter).normalized;

				// move on vector until 
				float movedistance = Mathf.Sqrt(Mathf.Pow(belowZero,2f) + Mathf.Pow(distancePointToCollisionGO,2f));

				newPositionWorld = newPositionWorld + (movedistance * CenterBottomToCollisionGO);

				this.transform.position = newPositionWorld;
			}

			*/

			this.transform.localPosition = RasterManager.Instance.Raster(this.transform.localPosition);

            // here check for possible snappings
            if (bottomFace.center.possibleSnappingVertexBundle != null)
            {
                if (!snapped)
                {
                    controllerForMovement.TriggerIfPressed(1500);
                    snapped = true;
                }

                if ((snapped && (newPositionCollider - lastPositionController).sqrMagnitude < 0.04f * transform.lossyScale.x))
                {
                    Vector3 distanceCurrentBottomSnap = bottomFace.center.possibleSnappingVertexBundle.transform.GetChild(0).position - bottomFace.center.transform.GetChild(0).position;
                    this.transform.position = this.transform.position + distanceCurrentBottomSnap;
                }
            }
			/*
            else if (bottomFace.center.possibleGroundSnapping != null)
            {
                if (!snapped)
                {
                    controllerForMovement.TriggerIfPressed(800);
                    snapped = true;
                }

                if ((snapped && (newPositionCollider - lastPositionController).sqrMagnitude < 0.02f * transform.lossyScale.x))
                {
                    Vector3 newPos = this.transform.position;
                    newPos.y = bottomFace.center.possibleGroundSnapping.transform.position.y + (transform.position.y - bottomFace.center.transform.GetChild(0).position.y);
                    this.transform.position = newPos;

                    lastPositionController = new Vector3(newPositionCollider.x, lastPositionController.y, newPositionCollider.z);
                }

            }*/
            else
            {
                snapped = false;
                lastPositionController = newPositionCollider;
            }

            if (group != null)
            {
                Vector3 distance = transform.position - prevPosition;
                group.Move(distance, this);
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

                            // change size depending on scale of stage
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
							lines.SetWidth (Mathf.Min(0.025f * transform.lossyScale.x, 0.03f), Mathf.Min(0.025f * transform.lossyScale.x, 0.03f));

                            if (snapped)
                            {
                                lines.SetColors(Color.green, Color.green);
                            }

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


			if (new Vector2 (transform.localPosition.x, transform.localPosition.z).magnitude > 3.6f) {
				if (!inTrashArea) {
					EnterTrashArea ();
				}
			} else {
				if (inTrashArea) {
					ExitTrashArea ();
				}
			}
        }
    }


    public void MoveModelingObject(Vector3 distance)
    {
        transform.position += distance;
    }


    public void Initiate(Mesh initialShape)
    {
        this.transform.GetChild(0).GetComponent<MeshFilter>().mesh = initialShape;

        mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        meshCollider = this.transform.GetChild(0).GetComponent<MeshCollider>();
        meshCollider.sharedMesh = initialShape;

        MeshCordinatesVertices = mesh.vertices;
        MeshTriangles = mesh.triangles;
        mesh.RecalculateNormals();

        MeshUV = mesh.uv;

        vertices = new Vertex[MeshCordinatesVertices.Length];

        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            GameObject vert = Instantiate(VertexPrefab);

            vert.transform.localPosition = MeshCordinatesVertices[i];
            vertices[i] = vert.GetComponent<Vertex>();

            // the normals in the mesh have the same Ids as the vertices, so we can safe a normal for every vertex
            vertices[i].normal = mesh.normals[i];
        }

        GetFacesBasedOnNormals();

        for (int i = 0; i < MeshCordinatesVertices.Length; i++)
        {
            if (MeshCordinatesVertices[i].y > 0.0f)
            {
                vertices[i].transform.SetParent(topFace.gameObject.transform);
            }
            else {
                vertices[i].transform.SetParent(bottomFace.gameObject.transform);
            }

            vertices[i].Initialize();
        }

        mesh.vertices = MeshCordinatesVertices;

        BundleSimilarVertices(topFace.gameObject.transform);
        BundleSimilarVertices(bottomFace.gameObject.transform);

        AssignVertexBundlesToFaces();

        for (int j = 0; j < faces.Length; j++)
        {
            faces[j].CalculateCenter();
        }

        InitializeVertexBundles();
        InitializeVertices();

        // if this is a square, we want to rotate it's coordinate system
        RotateAround(new Vector3(0f, 1f, 0f), 45f);
		initialPosition = transform.position;
		ShowOutline (false);
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


    public void InitializeVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Initialize();
        }
    }

    public void InitializeVertexBundles()
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            topFace.vertexBundles[i].Initialize();
        }

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            bottomFace.vertexBundles[i].Initialize();
        }
    }


    public void ShowNormals()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            Debug.DrawLine(faces[i].center.transform.position, faces[i].center.transform.position + faces[i].normal * 3.0f, Color.red, 500f);
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

                if (vertexBundle.transform.childCount == 0)
                {
                    Destroy(vertexBundle);
                }
                else {
                    face.GetComponent<Face>().AddVertexBundle(vertexBundle.GetComponent<VertexBundle>());
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

    public void PositionHandles()
    {
        handles.faceTopScale.transform.localPosition = topFace.scalerPosition;
        handles.faceBottomScale.transform.localPosition = bottomFace.scalerPosition;
        handles.CenterTopPosition.transform.localPosition = topFace.centerPosition;
        handles.CenterBottomPosition.transform.localPosition = bottomFace.centerPosition;
        handles.HeightTop.transform.localPosition = topFace.centerPosition;
        handles.HeightBottom.transform.localPosition = bottomFace.centerPosition;

        handles.RotationX.transform.localPosition = this.transform.localPosition;
        handles.RotationY.transform.localPosition = this.transform.localPosition;
        handles.RotationZ.transform.localPosition = this.transform.localPosition;
    }

    public void RotateHandles()
    {
        handles.faceTopScale.transform.localRotation = Quaternion.FromToRotation(handles.faceTopScale.transform.up, transform.TransformDirection(topFace.scalerPosition - topFace.centerPosition));
        handles.CenterTopPosition.transform.localRotation = Quaternion.FromToRotation(handles.CenterTopPosition.transform.up, transform.TransformDirection(topFace.normal));
        handles.HeightTop.transform.localRotation = Quaternion.FromToRotation(handles.HeightTop.transform.up, transform.TransformDirection(topFace.normal));

        handles.faceBottomScale.transform.localRotation = Quaternion.FromToRotation(handles.faceBottomScale.transform.up, transform.TransformDirection(bottomFace.scalerPosition - bottomFace.centerPosition));
        handles.CenterBottomPosition.transform.localRotation = Quaternion.FromToRotation(handles.CenterBottomPosition.transform.up, transform.TransformDirection(bottomFace.normal));
        handles.HeightBottom.transform.localRotation = Quaternion.FromToRotation(handles.HeightBottom.transform.up, transform.TransformDirection(bottomFace.normal));
    }

    public void InitiateHandles()
    {
        PositionHandles();
        RotateHandles();

        handles.faceTopScale.GetComponent<handle>().face = topFace;
        handles.faceBottomScale.GetComponent<handle>().face = bottomFace;

        handles.CenterTopPosition.GetComponent<handle>().face = topFace;
        handles.CenterBottomPosition.GetComponent<handle>().face = bottomFace;

        handles.HeightTop.GetComponent<handle>().face = topFace;
        handles.HeightBottom.GetComponent<handle>().face = bottomFace;

        topFace.centerHandle = handles.CenterTopPosition.GetComponent<handle>();
        topFace.heightHandle = handles.HeightTop.GetComponent<handle>();
        topFace.scaleHandle = handles.faceTopScale.GetComponent<handle>();

        bottomFace.centerHandle = handles.CenterBottomPosition.GetComponent<handle>();
        bottomFace.heightHandle = handles.HeightBottom.GetComponent<handle>();
        bottomFace.scaleHandle = handles.faceBottomScale.GetComponent<handle>();

    }

    public void Focus(Selection controller)
    {
        if (!focused)
        {
            Highlight();

            if (group != null)
            {
                group.FocusGroup(this);
            }

			if (!transform.parent.CompareTag("Library")){
				objectSelector.ShowSelectionButton (controller);
			}

            controller.AssignCurrentFocus(transform.gameObject);
            focused = true;
        }
    }

    public void UnFocus(Selection controller)
    {
		if (focused && !selected)
        {
            // ObjectSelector.SetActive(false);
			UnHighlight();

            if (group != null)
            {
                group.UnFocusGroup(this);
            }

			objectSelector.HideSelectionButton ();

            controller.DeAssignCurrentFocus(transform.gameObject);
            focused = false;
        }
    }

    public void Highlight()
    {
        Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
        transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r + 0.2f, newColor.g + 0.2f, newColor.b + 0.2f, 1f);
    }

    public void UnHighlight()
    {
        Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
        transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r - 0.2f, newColor.g - 0.2f, newColor.b - 0.2f, 1f);
    }

    public void Select(Selection controller, Vector3 uiPosition)
    {		
        if (group != null)
        {
            group.SelectGroup(this);
        }

		selected = true;

        controller.AssignCurrentSelection(transform.gameObject);
        handles.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        UiCanvasGroup.Instance.transform.position = uiPosition;
        UiCanvasGroup.Instance.OpenMainMenu(this, controller);

		ShowOutline(true);
    }
		
    public void DeSelect(Selection controller)
    {
        if (group != null)
        {
            group.DeSelectGroup(this);
        }

		selected = false;

		objectSelector.DeSelect (controller);
        controller.DeAssignCurrentSelection(transform.gameObject);
        handles.DisableHandles();

		UnFocus (controller);

        ShowOutline(false);
    }

    public void ShowOutline(bool value)
    {
        if (value)
        {
			// distance to object should be adapted to scale of
			transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.white);
			transform.GetChild (0).GetComponent<Renderer> ().material.SetFloat ("_Outline", 0.005f * this.transform.lossyScale.x);

			DisplayOutlineOfGroundFace ();
        }
        else
        {
			transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(1f,1f,1f,0f));
			transform.GetChild (0).GetComponent<Renderer> ().material.SetFloat ("_Outline", 0.00f);

			if (DistanceVisualisation != null) {
				// destroy previous distance vis
				foreach (Transform visualObject in DistanceVisualisation)
				{
					Destroy(visualObject.gameObject);
				}
			}
        }
    }

    public void AssignVertexBundlesToFaces()
    {

        // go through all vertices
        for (int i = 0; i < vertices.Length; i++)
        {

            // check normal of vertice and compare with normal of face
            for (int j = 0; j < faces.Length; j++)
            {

                // if normals are similar, linke parent vertex bundle to face
                if (vertices[i].normal == faces[j].normal)
                {
                    if (faces[j].typeOfFace == Face.faceType.SideFace)
                    {
                        faces[j].AddVertexBundle(vertices[i].transform.GetComponentInParent<VertexBundle>());
                    }
                }
            }
        }
    }


    public void StartMoving(Selection controller, ModelingObject initiater)
    {
        moving = true;
        controllerForMovement = controller;
        lastPositionController = controller.pointOfCollisionGO.transform.position;
        PositionOnMovementStart = transform.TransformPoint(bottomFace.center.coordinates);

		DisplayOutlineOfGroundFace ();
		objectSelector.HideSelectionButton ();

    }

	public void DisplayOutlineOfGroundFace(){
		// Display outline of groundface
		GroundVisualOnStartMoving = Instantiate(GroundVisualPrefab);
		GroundVisualOnStartMoving.transform.SetParent(DistanceVisualisation);
		LineRenderer lines = GroundVisualOnStartMoving.GetComponent<LineRenderer>();
		lines.SetVertexCount(bottomFace.vertexBundles.Length + 1);
		lines.SetWidth (Mathf.Min(0.025f * transform.lossyScale.x, 0.03f), Mathf.Min(0.025f * transform.lossyScale.x, 0.03f));

		for (int j = 0; j <= bottomFace.vertexBundles.Length; j++) {
			if (j == bottomFace.vertexBundles.Length) {
				Vector3 pos = bottomFace.vertexBundles [0].transform.GetChild (0).position;
				lines.SetPosition (j, pos);
			} else {
				Vector3 pos = bottomFace.vertexBundles [j].transform.GetChild (0).position;
				lines.SetPosition (j, pos);
			}
		}
	}

    public void StopMoving(Selection controller, ModelingObject initiater)
    {
        if (group != null && initiater == this)
        {
            group.StopMoving(controller, initiater);
        }

        firstTimeMoving = false;
        moving = false;
        controllerForMovement = null;

        // destroz previous distance vis
        foreach (Transform visualObject in DistanceVisualisation)
        {
            Destroy(visualObject.gameObject);
        }

		if (!transform.parent.CompareTag ("Library")) {
			objectSelector.ShowSelectionButton (controller);
		}
    }



    public void GetFacesBasedOnNormals()
    {

        //currently we have top and bottom face 2 times
        int arrayLength = 0;

        switch (typeOfObject)
        {
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
        for (int i = 0; i < vertices.Length; i++)
        {
            faceFound = null;

            // check for each vertex every face and 
            for (int j = 0; j < faces.Length; j++)
            {

                if (faces[j] != null && vertices[i].normal == faces[j].normal)
                {
                    faceFound = faces[j];
                }

            }

            if (faceFound == null)
            {
                GameObject newFace = Instantiate(FacePrefab);
                newFace.transform.SetParent(transform.GetChild(0));
                faces[faceCount] = newFace.GetComponent<Face>();

                // Check if it is top face? Or not create new face if it is the top/bottom face
                if (vertices[i].normal.x == 0 && vertices[i].normal.z == 0)
                {

                    switch (typeOfObject)
                    {
                        case ObjectType.triangle:
                            faces[faceCount].InitializeFace(3);
                            break;
                        case ObjectType.square:
                            faces[faceCount].InitializeFace(4);
                            break;
                        case ObjectType.hexagon:
                            faces[faceCount].InitializeFace(6);
                            break;
                        case ObjectType.octagon:
                            faces[faceCount].InitializeFace(8);
                            break;
                    }

                    //if direction is up, it is the top face (number of vertices depends on type)
                    if (vertices[i].normal.y > 0)
                    {
                        faces[faceCount].SetType(Face.faceType.TopFace);
                        topFace = newFace.GetComponent<Face>();

                        //if direction is down, it is the bottom face (number of vertices depends on type)
                    }
                    else {
                        faces[faceCount].SetType(Face.faceType.BottomFace);
                        bottomFace = newFace.GetComponent<Face>();
                    }

                    // others are side faces (4 vertices)
                }
                else {
                    faces[faceCount].InitializeFace(4);
                    faces[faceCount].SetType(Face.faceType.SideFace);
                }

                faces[faceCount].normal = vertices[i].normal;

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

    public Face GetFaceFromCollisionCoordinate(Vector3 pointOfCollision)
    {

        // use dot product to check if point lies on a face
        int idOfSmallest = -1;

        for (int i = 0; i < faces.Length; i++)
        {

            if (Mathf.Abs(Vector3.Dot((faces[i].center.transform.GetChild(0).transform.position - pointOfCollision), transform.TransformDirection(faces[i].normal))) <= 0.01)
            {
                idOfSmallest = i;
            }
        }

        if (idOfSmallest != -1)
        {
            return faces[idOfSmallest];
        }
        else
        {
            return null;
        }

    }

    public void StartScaling(bool initiater)
    {
        if (group == null)
        {
            relativeTo = GetBoundingBoxBottomCenter();
            initialDistancceCenterBottomScaler = scalerObject.coordinates - relativeTo;
        } else
        {
            if (initiater)
            {
                group.StartScalingGroup(this);
            }

            relativeTo = group.GetBoundingBoxBottomCenter();
            initialDistancceCenterBottomScaler = scalerObject.coordinates - relativeTo;
        }
    }



    public void ScaleBy(float newScale, bool initiater)
    {      
        if (group != null && initiater)
        { 
            group.ScaleBy(newScale, this);
        }

        Vector3 positionScalerObject = RasterManager.Instance.Raster(relativeTo + ((newScale) * initialDistancceCenterBottomScaler));
        Vector3 newDistanceCenterBottomScaler = positionScalerObject - relativeTo;

        float amount = (positionScalerObject - relativeTo).magnitude / (scalerObject.coordinates - relativeTo).magnitude;

        // get difference to last state to adjust other vertexbundles accordingly
        scalerObject.coordinates = positionScalerObject;

        bottomFace.ReplaceFacefromObjectScaler(relativeTo, amount);
        topFace.ReplaceFacefromObjectScaler(relativeTo, amount);
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

    public void TrashObject(bool initiator)
    {
        if (group != null && initiator)
        {
            group.TrashGroup(this);
        }

        transform.gameObject.SetActive(false);
        Trash.Instance.TrashAreaActive(false);
    }

    public void ChangeColor(Color color)
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = color;
    }

    public void RotateAround(Vector3 angleAxis, float angle)
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
            topFace.vertexBundles[i].coordinates = Quaternion.AngleAxis(angle, angleAxis) * topFace.vertexBundles[i].coordinates;
        }

        topFace.center.coordinates = Quaternion.AngleAxis(angle, angleAxis) * topFace.center.coordinates;

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
            bottomFace.vertexBundles[i].coordinates = Quaternion.AngleAxis(angle, angleAxis) * bottomFace.vertexBundles[i].coordinates;

        }

        bottomFace.center.coordinates = Quaternion.AngleAxis(angle, angleAxis) * bottomFace.center.coordinates;

        // update centers and recalculate normals of side faces

        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
            faces[i].RecalculateNormal();
            faces[i].UpdateSpecialVertexCoordinates();
        }

        handles.HeightTop.transform.RotateAround(new Vector3(0f, 0f, 0f), angleAxis, angle);
        handles.HeightBottom.transform.RotateAround(new Vector3(0f, 0f, 0f), angleAxis, angle);
        handles.faceTopScale.transform.RotateAround(new Vector3(0f, 0f, 0f), angleAxis, angle);
        handles.faceBottomScale.transform.RotateAround(new Vector3(0f, 0f, 0f), angleAxis, angle);

        // update inner coordinate system
        coordinateSystem.transform.RotateAround(new Vector3(0f, 0f, 0f), angleAxis, angle);
    }

    public void SetVertexBundlePositions(ModelingObject otherObject)
    {
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            topFace.vertexBundles[i].coordinates = otherObject.topFace.vertexBundles[i].coordinates;
        }

        topFace.center.coordinates = otherObject.topFace.center.coordinates;

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            // rotate coordinates of every vertexbundle
            bottomFace.vertexBundles[i].coordinates = otherObject.bottomFace.vertexBundles[i].coordinates;

        }

        bottomFace.center.coordinates = otherObject.bottomFace.center.coordinates;

        // update centers and recalculate normals of side faces

        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].UpdateCenter();
            faces[i].RecalculateNormal();
            faces[i].UpdateSpecialVertexCoordinates();
        }

        handles.HeightTop.transform.localEulerAngles = otherObject.handles.HeightTop.transform.localEulerAngles;
        handles.HeightBottom.transform.localEulerAngles = otherObject.handles.HeightBottom.transform.localEulerAngles;
        handles.faceTopScale.transform.localEulerAngles = otherObject.handles.faceTopScale.transform.localEulerAngles;
        handles.faceBottomScale.transform.localEulerAngles = otherObject.handles.faceBottomScale.transform.localEulerAngles;

        // update inner coordinate system
        coordinateSystem.transform.localEulerAngles = otherObject.coordinateSystem.transform.localEulerAngles;
    }

    public Vector3 GetBoundingBoxTopCenter()
    {
        Vector3 boundingBoxTopCenter = Vector3.zero;
        float highestYvalue = -9999f;

        // Calculate Center of Vertices
        Vector3 center = GetBoundingBoxCenter();

        // find Vertex with highest y value
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            if (topFace.vertexBundles[i].coordinates.y > highestYvalue)
            {
                highestYvalue = topFace.vertexBundles[i].coordinates.y;
            }
        }

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            if (bottomFace.vertexBundles[i].coordinates.y > highestYvalue)
            {
                highestYvalue = bottomFace.vertexBundles[i].coordinates.y;
            }
        }

        // return point with highest y value and x/z of center
        Vector3 boundingBoxBottomCenter = new Vector3(center.x, highestYvalue, center.z);

        return boundingBoxTopCenter;
    }

    public Vector3 GetBoundingBoxBottomCenter()
    {        
        float lowestYvalue = 9999f;

        // Calculate Center of Vertices
        Vector3 center = GetBoundingBoxCenter();

        // find Vertex with lowest y value
        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            if (topFace.vertexBundles[i].coordinates.y < lowestYvalue)
            {
                lowestYvalue = topFace.vertexBundles[i].coordinates.y;
            }
        }

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            if (bottomFace.vertexBundles[i].coordinates.y < lowestYvalue)
            {
                lowestYvalue = bottomFace.vertexBundles[i].coordinates.y;
            }
        }

        // return point with lowest y value and x/z of center
        Vector3 boundingBoxBottomCenter = new Vector3(center.x, lowestYvalue, center.z);

        return boundingBoxBottomCenter;
    }

    public Vector3 GetBoundingBoxCenter()
    {
        Vector3 boundingBoxCenter = Vector3.zero;

        for (int i = 0; i < topFace.vertexBundles.Length; i++)
        {
            boundingBoxCenter += topFace.vertexBundles[i].coordinates;
        }

        for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
        {
            boundingBoxCenter += bottomFace.vertexBundles[i].coordinates;
        }

        boundingBoxCenter = boundingBoxCenter / (topFace.vertexBundles.Length + bottomFace.vertexBundles.Length);

        return boundingBoxCenter;
    }

	public Vector3 GetPosOfClosestVertex(Vector3 position){
		Vector3 closestVertex = bottomFace.vertexBundles[0].coordinates;
		float shortestDistance = 999999f;

		for (int i = 0; i < bottomFace.vertexBundles.Length; i++)
		{
			Vector3 newCoordinate = transform.TransformPoint (bottomFace.vertexBundles [i].coordinates);

			if (Vector3.Distance (position, newCoordinate) < shortestDistance) {
				closestVertex =  newCoordinate;
				shortestDistance = Vector3.Distance (position,  newCoordinate);
			}
		}

		return closestVertex;

	}

	public void EnterTrashArea(){
		inTrashArea = true;

		Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r - 0.3f, newColor.g - 0.3f, newColor.b - 0.3f, 1f);

		// display trash icon
		LeanTween.alpha(trashIcon, 1f, 0.3f);
	}

	public void ExitTrashArea(){
		inTrashArea = false;

		Color newColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(newColor.r + 0.3f, newColor.g + 0.3f, newColor.b + 0.3f, 1f);

		// display trash icon
		LeanTween.alpha(trashIcon, 0f, 0.3f);
	}



}
		

 
 