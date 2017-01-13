﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class handle : MonoBehaviour {

    public enum handleType
    {
        ScaleFace,
        PositionCenter,
        Height,
        Rotation,
		ScaleX,
		ScaleY,
		ScaleZ,
		ScaleMinusX,
		ScaleMinusY,
		ScaleMinusZ,
		MoveY, 
		UniformScale,
		RotateTranslate, 
		RotationHandleToggle
    };

    public GameObject connectedObject;
    private ModelingObject connectedModelingObject;

    public handleType typeOfHandle;
    public GameObject colliderHandle;
    public handles handles;
    public Face face;
    public Transform p1;
    public Transform p2;
    private bool clicked;
	public bool focused = false;
    private Vector3 lastPosition;
    private Vector3 initialLocalPositionFace;
    private Vector3 initialLocalPositionHandle;
    private Vector3 initialPositionHandle;
	private Vector3 initialOffsetToHandle;
    private Vector3 initialDistancceCenterScaler;
    private bool resetLastPosition = true;

    public Transform RotationAxis;

    private Vector3 directionHandle;
    private float lastInput = 0f;

    private bool rotateStep = false;
    private bool locked = false;

	public Transform circle;
	private Vector3 initialScaleCircle;

	public GameObject arrow;
	public GameObject rotationArrow;
	private Vector3 initialSizeArrow;
    private bool rotating = false;
    private bool newRotation = false;
    private float prevRotationAmount;
	private float firstRotationAmount;

	private bool newScaling = false;
	private bool newMovement = false;
	private float prevScalingAmount;

	public Color normalColor;
	public Color hoverColor;

	private Vector3 centerOfScaling = new Vector3(0f,0f,0f);
	private Vector3 touchPointForScaling = new Vector3(0f,0f,0f);

	public Vector3 boundingBoxCorner1;
	public Vector3 boundingBoxCorner2;

	private float smoothTime = 0.03f;
	private float velocity = 0.0f;

	private float initialScale;

	public GameObject rotationVisualPrefab;
	private GameObject rotationVisual;
	private Vector3 lastIntersectionPoint;
	private Vector3 firstIntersectionPoint;
	private float smoothTimeRotation = 0.12f;
	private Vector3 velocityRotation = Vector3.zero;
	private GameObject RotationOnStartLine;
	private GameObject CurrentRotationLine;

	public GameObject arrowPart1;
	public GameObject arrowPart2;
	public GameObject arrowPart3;

	// rotationParameters
	private Vector3 p1Rotation;
	private Vector3 p2Rotation;
	private Vector3 rotationAxis;
	private Plane currentPlane;
	private Plane currentPlaneOtherDirection;
	private Vector3 centerOfRotation = Vector3.zero;
	private Vector3 rotationRelativeTo;

	public GameObject bottomPartSphere;
	public GameObject topPartSphere;

	private Vector3 standardPosBottom;
	private Vector3 OpenPosBottom;
	private Vector3 FocusedPosBottom;

	bool reCheckRotationDirection;
	float rotationDirection = 1f;

	private float initialInput;


    // Use this for initialization
    void Start () {
		if (arrow != null) {
			// Hover effect: Scale bigger & change color
			LeanTween.color(arrow, normalColor, 0.06f);

			if (rotationArrow != null) {
				LeanTween.color (rotationArrow, normalColor, 0.06f);
			}
		}

        ResetLastPosition();
        connectedModelingObject = connectedObject.GetComponent<ModelingObject>();

		if (arrow != null) {
			initialSizeArrow = arrow.transform.localScale;
		}

		if (typeOfHandle == handleType.RotationHandleToggle) {
			standardPosBottom = bottomPartSphere.transform.localPosition;
		}
		//initialScale = transform.lossyScale.x;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		// adapt scaling like in other prototype

		if (typeOfHandle != handleType.Rotation) {
			float distanceToCamera = (Camera.main.transform.position - transform.position).magnitude;
			Vector3 size = Vector3.one * distanceToCamera * 0.35f;

			transform.localScale = size;
		}
    }

	private float CalculateInputFromPoint(Vector3 pointOfCollision, Vector3 pos1, Vector3 pos2)
    {

        if (resetLastPosition)
        {
			directionHandle = (pos2 - pos1).normalized;

            if (face != null)
            {
                initialLocalPositionFace = face.center.coordinates;
                initialDistancceCenterScaler = face.scalerPosition - face.centerPosition;
            }

            initialLocalPositionHandle = transform.localPosition;
            
			//initialOffsetToHandle = TouchElements.Instance.YMoveHandle.transform.position - transform.position;
			initialPositionHandle = transform.position;
        }

		Vector3 pq = pointOfCollision - pos1;
        Vector3 newPoint = p1.transform.position + (directionHandle * (Vector3.Dot(pq, directionHandle) / directionHandle.sqrMagnitude));

        if (resetLastPosition)
        {
            resetLastPosition = false;
            lastPosition = newPoint;
        }

        float input = (newPoint - lastPosition).magnitude;

        // check direction of vector:
        if (Vector3.Dot((newPoint - lastPosition), directionHandle) < 0f)
        {
            input = input * (-1f);
        } 

        return input;

    }



    private Vector3 ProjectPointOnPlane(GameObject pointOfCollision)
    {
        Vector3 normal =  Vector3.Normalize(p2.transform.position - p1.transform.position);
        Vector3 point = pointOfCollision.transform.position;
        float dist = Vector3.Dot((point - p1.transform.position), normal);

        Vector3 projectedPoint = point - (dist * normal);

        return projectedPoint;
    }

    private Vector3 calculate3DInputFromPoint(GameObject pointOfCollision)
    {
        Vector3 newPoint = ProjectPointOnPlane(pointOfCollision);

        if (resetLastPosition)
        {
            lastPosition = newPoint;
            resetLastPosition = false;
        }

        Vector3 DistanceVector = newPoint - lastPosition;
        lastPosition = newPoint;

        return DistanceVector;
    }

	public bool ApplyChanges(GameObject pointOfCollision, bool alreadyMoving, Selection controller)
    {
		if (!alreadyMoving) {
			//connectedModelingObject.HideBoundingBox (false);
		}

        switch (typeOfHandle)
        {
            case handleType.ScaleFace:
                ScaleFace(pointOfCollision);
                break;
            case handleType.PositionCenter:
                MoveCenterPosition(pointOfCollision);
                break;
            case handleType.Height:
                ChangeHeight(pointOfCollision);
                break;
			case handleType.Rotation:
				if (!alreadyMoving) {
					handles.HideRotationHandlesExcept (null);
					connectedModelingObject.CalculateBoundingBox();
					connectedModelingObject.CalculateBoundingBoxWorld();
					handles.HideScalingHandlesExcept (null);
					TouchElements.Instance.Hide ();
					newRotation = true;
				}   

				Rotate (pointOfCollision, controller, true);
				
				connectedModelingObject.PositionHandles (true);
				connectedModelingObject.RotateHandles ();
			    handles.HideRotationHandlesExcept (null);
			    handles.HideScalingHandlesExcept (this);

                break;
			case handleType.ScaleX:
				if (!alreadyMoving) {
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}         
				ScaleNonUniform (pointOfCollision, new Vector3(1f,0f,0f));
				break;
			case handleType.ScaleY:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3(0f,1f,0f));
				break;
			case handleType.ScaleZ:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3(0f,0f,1f));
				break;
			case handleType.ScaleMinusX:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3(1f,0f,0f));
				break;
			case handleType.ScaleMinusY:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3(0f,1f,0f));
				break;
			case handleType.ScaleMinusZ:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3(0f,0f,1f));
				break;
			case handleType.MoveY:
				if (!alreadyMoving) {
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newMovement = true;	
				}
				MoveYPosition (pointOfCollision);
				handles.HideRotationHandlesExcept (null);
				handles.HideScalingHandlesExcept (this);
				break;
			case handleType.UniformScale:
				Debug.Log ("already moving " + alreadyMoving);
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleUniform (pointOfCollision);
				handles.HideRotationHandlesExcept (null);
				handles.HideScalingHandlesExcept (this);
				break;	

		case handleType.RotationHandleToggle:
				//handles.ToggleOnOffRotationHandles ();
				break;
        }

		connectedModelingObject.RecalculateSideCenters();
		connectedModelingObject.RecalculateNormals();

		//handles.HideRotationHandlesExcept (null);
		//handles.HideScalingHandlesExcept (this);


		//connectedModelingObject.ShowBoundingBox ();
		//connectedModelingObject.ShowBoundingBox ();

		return true;
    }

	public void moveAndRotate(GameObject pointOfCollision, Selection controller){
		connectedModelingObject.StartMoving (controller, connectedModelingObject);
		//Rotate (pointOfCollision, controller, false);
	}

	public void ScaleUniform(GameObject pointOfCollision){
		connectedModelingObject.CalculateBoundingBox ();
		connectedModelingObject.boundingBox.DrawBoundingBox ();

		//float input = CalculateInputFromPoint(pointOfCollision.transform.position, p1.transform.position, p2.transform.position);
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + transform.forward);

		centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxBottomCenterWorld());
		touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxTopCenter ());

		if (newScaling)
		{
			Debug.Log ("new scaling");
			prevScalingAmount = input;
			newScaling = false;
		}

		Debug.Log ("new amount: " + prevScalingAmount + " and input at " + input);

		// check if we can also raster just the touchpoint
		centerOfScaling = RasterManager.Instance.Raster(centerOfScaling);

		float RasteredDistanceCenters = Mathf.Max(RasterManager.Instance.Raster((centerOfScaling - touchPointForScaling).magnitude * Mathf.Abs(1f + (input - prevScalingAmount))), RasterManager.Instance.rasterLevel * 2);
		input = (RasteredDistanceCenters / ((centerOfScaling - touchPointForScaling).magnitude)) - 1f + prevScalingAmount;

		float adjustedInput = Mathf.Abs (1f + (input - prevScalingAmount));

		//Debug.Log ("new scale is " + adjustedInput);

		connectedModelingObject.ScaleNonUniform (adjustedInput, new Vector3(1f,1f,1f), typeOfHandle, centerOfScaling);

		//connectedModelingObject.ScaleBy (adjustedInput, true);

		prevScalingAmount = input;
	}

	public void ScaleNonUniform(GameObject pointOfCollision, Vector3 direction){
		
		connectedModelingObject.CalculateBoundingBox ();
		connectedModelingObject.boundingBox.DrawBoundingBox ();

	//	Vector3 normalizedDirection = direction.normalized;

		float input = CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + transform.forward);

		// get current distance 
		switch (typeOfHandle)
		{
		case handle.handleType.ScaleX:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxLeftCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxRightCenter ());
			break;
		case handle.handleType.ScaleY:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxBottomCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxTopCenter ());
			break;		
		case handle.handleType.ScaleZ:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxFrontCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxBackCenter ());
			break;		
		case handle.handleType.ScaleMinusX:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxRightCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxLeftCenter ());
			break;
		case handle.handleType.ScaleMinusY:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxTopCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxBottomCenter ());
			break;
		case handle.handleType.ScaleMinusZ:
			centerOfScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxBackCenter ());
			touchPointForScaling = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxFrontCenter ());
			break;
		}

		if (newScaling)
		{
			prevScalingAmount = input;
			newScaling = false;
		}

		//input = Mathf.SmoothDamp(prevScalingAmount, input, ref velocity, smoothTime);

		// check if we can also raster just the touchpoint
		centerOfScaling = RasterManager.Instance.Raster(centerOfScaling);

		//touchPointForScaling = RasterManager.Instance.Raster(touchPointForScaling);

		float RasteredDistanceCenters = Mathf.Max(RasterManager.Instance.Raster((centerOfScaling - touchPointForScaling).magnitude * Mathf.Abs(1f + (input - prevScalingAmount))), RasterManager.Instance.rasterLevel * 2);
		input = (RasteredDistanceCenters / ((centerOfScaling - touchPointForScaling).magnitude)) - 1f + prevScalingAmount;

		float adjustedInput = Mathf.Abs (1f + (input - prevScalingAmount));

		connectedModelingObject.ScaleNonUniform (adjustedInput, direction.normalized, typeOfHandle, centerOfScaling);

		// correct center if object is off grid
		// check how far off grid
		//Vector3 center = connectedModelingObject.transform.InverseTransformPoint(connectedModelingObject.GetBoundingBoxCenter());
		//Vector3 correctedCenter = RasterManager.Instance.Raster(center);

		//connectedModelingObject.transform.localPosition = connectedModelingObject.transform.localPosition + (correctedCenter-center);

		prevScalingAmount = input;


	}

	public void FinishUsingHandle(Selection controller){
		if (typeOfHandle != handleType.Height && typeOfHandle != handleType.ScaleFace) {
			if (handles.rotationHandlesvisible || (ModeChanger.Instance.modeChangerActive && ModeChanger.Instance.currentMode == ModeChanger.BuildingMode.RotationMode)) {
				handles.ShowRotationHandles();
			}
			handles.ShowNonUniformScalingHandles();
		}

		if (typeOfHandle == handleType.RotateTranslate){
			connectedModelingObject.StopMoving (controller, connectedModelingObject);

			// destroz previous distance vis
			foreach (Transform visualObject in connectedModelingObject.DistanceVisualisation)
			{
				Destroy(visualObject.gameObject);
			}
		}

		if (typeOfHandle == handleType.MoveY) {
			// destroz previous distance vis
			foreach (Transform visualObject in connectedModelingObject.DistanceVisualisation)
			{
				Destroy(visualObject.gameObject);
			}
		}

		if (typeOfHandle == handleType.RotationHandleToggle) {
			handles.ToggleOnOffRotationHandles ();
		}

		if (rotationVisual != null) {
			Destroy (rotationVisual);
			Destroy (CurrentRotationLine);
			Destroy (RotationOnStartLine);
		}

		VisualizeAngle.Instance.Hide ();

		connectedModelingObject.ShowBoundingBox (false);

		Vector3 newScale = Vector3.one;
		float highestDistanceToCenter = connectedModelingObject.boundingBoxWorld.getMaxDistanceinBB(connectedModelingObject.GetBoundingBoxCenterWorld());

		/*
		if ((connectedModelingObject.GetBoundingBoxCenter () - connectedModelingObject.GetBoundingBoxFrontCenter()).magnitude > (connectedModelingObject.GetBoundingBoxCenter () - connectedModelingObject.GetBoundingBoxLeftCenter ()).magnitude) {
			newScale = Vector3.one * (connectedModelingObject.GetBoundingBoxCenter () - connectedModelingObject.GetBoundingBoxFrontCenter()).magnitude * 3f;
		} else {
			newScale = Vector3.one * (connectedModelingObject.GetBoundingBoxCenter () - connectedModelingObject.GetBoundingBoxLeftCenter()).magnitude * 3f;
		}*/

		handles.RotateX.transform.localScale = newScale * highestDistanceToCenter * 2f;
		handles.RotateY.transform.localScale = newScale * highestDistanceToCenter * 2f;
		handles.RotateZ.transform.localScale = newScale * highestDistanceToCenter * 2f;

		handles.groundPlane.transform.localScale = newScale * highestDistanceToCenter * 0.5f;

		TouchElements.Instance.UnfocusAll ();
	}

    private void ScaleFace(GameObject pointOfCollision)
    {
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + transform.forward) * 1.5f;

       // Raster
		float RasteredDistanceCenterScaler = RasterManager.Instance.Raster((1f - input) * (initialDistancceCenterScaler).magnitude);
		input = 1f - (RasteredDistanceCenterScaler / (initialDistancceCenterScaler).magnitude);

		Vector3 positionScaler = initialLocalPositionFace + ((1f - input) * initialDistancceCenterScaler);
        Vector3 newDistanceCenterScaler = positionScaler - face.centerPosition;

		if (newDistanceCenterScaler.magnitude >= RasterManager.Instance.rasterLevel && Vector3.Dot(initialDistancceCenterScaler, newDistanceCenterScaler)>0)
        {
			positionScaler = positionScaler;
			transform.position = connectedModelingObject.transform.TransformPoint(positionScaler);
			face.scaler.coordinates = positionScaler;  
        }

        face.UpdateScaleFromCorner();
    }

    private void MoveCenterPosition(GameObject pointOfCollision)
    {
        Vector3 input = RasterManager.Instance.Raster(calculate3DInputFromPoint(pointOfCollision));
        
        colliderHandle.transform.parent.position += input;
        face.center.coordinates += input;
    }

    public void ResetLastPosition()
    {
        resetLastPosition = true;
        lastInput = 0f;
    }

    private void ChangeHeight(GameObject pointOfCollision)
    {
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + transform.forward);

		float RasteredLength = RasterManager.Instance.Raster(((input) * (face.normal).magnitude));
		input = RasteredLength / (face.normal).magnitude;

        Vector3 position = initialLocalPositionHandle + ((-input) * face.normal);
        Vector3 positionFace = initialLocalPositionFace + ((-input) * face.normal);
    
        // check that center does not get below other center

        if ((face.typeOfFace == Face.faceType.TopFace && positionFace.y > face.parentModelingObject.bottomFace.centerPosition.y) || (face.typeOfFace == Face.faceType.BottomFace && positionFace.y < face.parentModelingObject.topFace.centerPosition.y))
        {
            transform.localPosition = position;
            face.center.coordinates = positionFace;
            face.UpdateFaceFromCenter();
        }
    }

	private void MoveYPosition (GameObject pointOfCollision){

		if (newMovement) {
			newMovement = false;
			connectedModelingObject.CalculateBoundingBox ();

			connectedModelingObject.initialCoordinatesBoundingBox = new Vector3[4];

			for (int j = 0; j < 4; j++)
			{
				connectedModelingObject.initialCoordinatesBoundingBox[j] = connectedModelingObject.boundingBox.coordinates[j+4];
			}

			connectedModelingObject.PositionOnMovementStart = 0.25f * connectedModelingObject.boundingBox.coordinates[4] + 0.25f * connectedModelingObject.boundingBox.coordinates[5] + 0.25f * connectedModelingObject.boundingBox.coordinates[6] + 0.25f * connectedModelingObject.boundingBox.coordinates[7];
		
			initialOffsetToHandle = transform.position - pointOfCollision.transform.position;

			//Debug.Log ("initial offset" + initialOffsetToHandle + "transform.position at " + transform.position + " point of collision " + pointOfCollision.transform.position);

			//Debug.Log ("new handle griff");

			//initialInput = CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + transform.forward);
			//Debug.Log("Set initial input at" + initialInput);

			//float RasteredLengthInitial = RasterManager.Instance.Raster(((initialInput) * (Vector3.up).magnitude));
			//initialInput = RasteredLengthInitial / (Vector3.up).magnitude;
		}

		// destroy previous distance vis
		foreach (Transform visualObject in connectedModelingObject.DistanceVisualisation){
			Destroy (visualObject.gameObject);
		}

		float input = CalculateInputFromPoint((pointOfCollision.transform.position + initialOffsetToHandle), transform.position, transform.position + transform.forward);

		float RasteredLength = RasterManager.Instance.Raster(((input) * (Vector3.up).magnitude));
		input = RasteredLength / (Vector3.up).magnitude;

		Vector3 position = initialPositionHandle + ((input) * Vector3.up);

	//	Debug.Log ("input is " + input + " minus initial input: " + (input - initialInput) + "with initial input: " + initialInput);

		Vector3 distance = position - transform.position;

		Vector3 prevPos = connectedModelingObject.transform.position;

		connectedModelingObject.transform.position = connectedModelingObject.transform.position + distance;

		// keep above 0
		connectedModelingObject.KeepAboveZero(prevPos);

		//connectedModelingObject.VisualizeMovement ();
	}

	private void Rotate(GameObject pointOfCollision, Selection controller, bool visual)
    {
		// fix centers etc

		//Debug.Log ("rotate");

		if (newRotation) {
			VisualizeAngle.Instance.Show();

			connectedModelingObject.HideBoundingBox (false);

			// center object on raster

			/*
			Vector3 center = connectedModelingObject.transform.InverseTransformPoint(connectedModelingObject.GetBoundingBoxBottomCenter());
			Vector3 correctedCenter = RasterManager.Instance.Raster(center);

			connectedModelingObject.transform.localPosition = connectedModelingObject.transform.localPosition + (correctedCenter-center);
			*/ 

			// define rotation axis
			p1Rotation = connectedModelingObject.GetBoundingBoxCenter();
			p2Rotation = p1Rotation + (p1.transform.position - p2.transform.position);

			rotationAxis = connectedModelingObject.transform.InverseTransformDirection(p2Rotation - p1Rotation);

			currentPlane = new Plane ((p1Rotation-p2Rotation).normalized, transform.position);
			currentPlaneOtherDirection = new Plane ((-1f) * (p1Rotation-p2Rotation).normalized, transform.position);

			// get center of rotaion:
			Ray rayCenterToPlane = new Ray (p1Rotation, (p1Rotation-p2Rotation).normalized * (-1f));
			Ray rayCenterToPlaneOtherDir = new Ray (p1Rotation, (p1Rotation-p2Rotation).normalized);

			float rayDistanceToCenter;

			if (currentPlane.Raycast (rayCenterToPlane, out rayDistanceToCenter)) {
				centerOfRotation = rayCenterToPlane.GetPoint (rayDistanceToCenter);
			} else if (currentPlaneOtherDirection.Raycast (rayCenterToPlaneOtherDir, out rayDistanceToCenter)) {
				centerOfRotation = rayCenterToPlaneOtherDir.GetPoint (rayDistanceToCenter);
			}	

			centerOfRotation = transform.position;
		}

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		float rayDistance;
		float newRotationAmount = 0f;

		if (currentPlane.Raycast (ray, out rayDistance)) {

			if (rayDistance < 25f) {
				if (newRotation) {
					lastIntersectionPoint = ray.GetPoint (rayDistance);

					// fix point
					rotationRelativeTo = lastIntersectionPoint;
				}

				//Vector3 intersectionPoint = Vector3.SmoothDamp (lastIntersectionPoint, ray.GetPoint (rayDistance), ref velocityRotation, smoothTimeRotation);
				Vector3 intersectionPoint = ray.GetPoint (rayDistance);

				Vector3 adjustedIntersectPoint = p1Rotation + (intersectionPoint - p1Rotation).normalized * 1.5f;

				float direction = Mathf.Sign (Vector3.Dot (Vector3.Cross (rotationRelativeTo - p1Rotation, adjustedIntersectPoint - p1Rotation), currentPlane.normal));

				lastIntersectionPoint = intersectionPoint;

				// to do: check Angle to center on point of intersection
				newRotationAmount = Vector3.Angle (rotationRelativeTo - centerOfRotation, intersectionPoint - centerOfRotation) * direction * (-1f);
			}

		} else if (currentPlaneOtherDirection.Raycast (ray, out rayDistance)) {	

			if (rayDistance < 25f) {
				if (newRotation) {
					lastIntersectionPoint = ray.GetPoint (rayDistance);
				}

				Vector3 intersectionPoint = Vector3.SmoothDamp(lastIntersectionPoint, ray.GetPoint (rayDistance), ref velocityRotation, smoothTimeRotation);
				Vector3 adjustedIntersectPoint = p1Rotation + (intersectionPoint - p1Rotation).normalized * 1.5f;

				float direction = Mathf.Sign (Vector3.Dot (Vector3.Cross (rotationRelativeTo - p1Rotation, adjustedIntersectPoint - p1Rotation), currentPlaneOtherDirection.normal));

				lastIntersectionPoint = intersectionPoint;

				// to do: check Angle to center on point of intersection
				newRotationAmount = Vector3.Angle (rotationRelativeTo - centerOfRotation, intersectionPoint - centerOfRotation) * direction;
			}
		}

		if (newRotationAmount < 0) {
			newRotationAmount = 360f + newRotationAmount;
		}

		if (visual) {
			if (rotationVisual == null) {
				rotationVisual = Instantiate (rotationVisualPrefab);
				rotationVisual.transform.localScale = Vector3.one * (rotationRelativeTo - centerOfRotation).magnitude * 1.85f;	
				rotationVisual.GetComponent<RotationVisual> ().ColorCircle (normalColor);

				RotationOnStartLine = Instantiate (handles.linesGO);
				CurrentRotationLine = Instantiate (handles.linesGO);
				firstIntersectionPoint = lastIntersectionPoint;
			}

			rotationVisual.transform.position = centerOfRotation;
			rotationVisual.transform.rotation = Quaternion.LookRotation (p2Rotation - p1Rotation);
		}

		float count = Mathf.Round(newRotationAmount / 90f);

		float distanceToCenter = (lastIntersectionPoint - centerOfRotation).magnitude / (rotationRelativeTo - centerOfRotation).magnitude;

		float disToCenter = (lastIntersectionPoint - centerOfRotation).magnitude;
		float maxDistance = connectedModelingObject.boundingBox.getMaxDistanceinBB (centerOfRotation);

		//float circleSize = (transform.position - connectedModelingObject.GetBoundingBoxCenter ()).magnitude * 2f;	

		//Debug.Log ("distance at " + maxDistance / disToCenter);

		//if (maxDistance/disToCenter > 0.75f) {
			
		if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
			newRotationAmount = RasterManager.Instance.RasterAngle (newRotationAmount);
			//newRotationAmount = RasterManager.Instance.RasterAngleSmooth (newRotationAmount);
		} else {
			newRotationAmount = RasterManager.Instance.RasterAngleSmooth (newRotationAmount);
		}
			
		if (newRotation) {
			prevRotationAmount = newRotationAmount;
			firstRotationAmount = newRotationAmount;
			newRotation = false;
			reCheckRotationDirection = true;
		}

		float shortestRotationAmount = newRotationAmount - prevRotationAmount;

		if (shortestRotationAmount > 180) {
			shortestRotationAmount = shortestRotationAmount - 360f;
		} 		 

		connectedModelingObject.RotateAround(rotationAxis.normalized, shortestRotationAmount, connectedModelingObject.transform.InverseTransformPoint (p1Rotation));

		/*
		CurrentRotationLine.GetComponent<Lines> ().DrawLinesWorldCoordinate (new Vector3[] {
			centerOfRotation,
			lastIntersectionPoint
		}, 0);
*/


		float showingValue = newRotationAmount - firstRotationAmount;


		if (showingValue == 360f) {
			showingValue = 0f;
		}

		if (showingValue <= 5f && showingValue >= -5f) {
			reCheckRotationDirection = true;
		}

		if (reCheckRotationDirection) {
			if (showingValue > 180f) {
				showingValue = showingValue - 360f;
				reCheckRotationDirection = false;
			} else if (showingValue < -180f) {
				showingValue = showingValue + 360f;
				reCheckRotationDirection = false;
			} 

			if (showingValue < 0f) {
				rotationDirection = -1f;
			} else {
				rotationDirection = 1f;
			}
		}			

		if (rotationDirection == -1f && showingValue > 0f) {
			showingValue = showingValue - 360f;
		} else if (rotationDirection == 1f && showingValue < 0f) {
			showingValue = showingValue + 360f;
		}



		VisualizeAngle.Instance.SetNumber (showingValue);

		//Debug.Log ("Distace bei " + (maxDistance / disToCenter));

		if (visual) {
			//if (maxDistance/disToCenter > 0.75f) {
			
			if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
				//rotationVisual.GetComponent<RotationVisual> ().RotationVisualisation (firstIntersectionPoint, lastIntersectionPoint, true);
				rotationVisual.GetComponent<RotationVisual> ().RotationVisualisation (firstIntersectionPoint, lastIntersectionPoint, false, showingValue);
			} else {
				rotationVisual.GetComponent<RotationVisual> ().RotationVisualisation (firstIntersectionPoint, lastIntersectionPoint, true, showingValue);
			}
		}

       	prevRotationAmount = newRotationAmount;    

    }

    private void SetRotateStepTrue()
    {
        rotateStep = true;
    }

   
    public void updateHandlePosition()
    {
        Vector3 rotation = connectedObject.transform.localRotation.eulerAngles;
        transform.Rotate(rotation);
    }

    public void Focus(Selection controller)
    {
		//Debug.Log ("Focus handle");

		if (!focused) {

			if (!clicked && !handles.objectFocused)	{

				if (arrow != null) {
					// Hover effect: Scale bigger & change color

					if (typeOfHandle != handleType.Rotation) {
						LeanTween.scale (arrow, new Vector3 (initialSizeArrow.x * 1.1f, initialSizeArrow.y * 1.1f, initialSizeArrow.z * 1.1f), 0.06f);
					}
					LeanTween.color (arrow, hoverColor, 0.06f);
				}

				if (rotationArrow != null) {
					LeanTween.color (rotationArrow, hoverColor, 0.06f);
				}

				if (typeOfHandle == handleType.Rotation) {
					LeanTween.color (arrowPart1, hoverColor, 0.06f);
					LeanTween.color (arrowPart2, hoverColor, 0.06f);
					LeanTween.color (arrowPart3, hoverColor, 0.06f);
					//handles.HideRotationHandlesExcept(this);
				}

				if (typeOfHandle == handleType.RotationHandleToggle) {
					//LeanTween.move(topPartSphere, topPartSphere.transform.position + new Vector3(0f,0.02f,0f), 0.2f).setEase(LeanTweenType.easeOutQuad);
					LeanTween.moveLocal(bottomPartSphere, bottomPartSphere.transform.localPosition + new Vector3(0f,-0.02f,0f), 0.2f).setEase(LeanTweenType.easeOutQuad);
				}

                controller.AssignCurrentFocus(transform.gameObject);
				handles.objectFocused = true;

				focused = true;
			}
		}

    }

    public void UnFocus(Selection controller)
    {
		if (focused) {
			if(!controller.triggerPressed)
			{
				if (arrow != null) {
					// Hover effect: Scale bigger & change color
					LeanTween.scale(arrow, new Vector3 (initialSizeArrow.x, initialSizeArrow.y, initialSizeArrow.z), 0.06f);
					LeanTween.color(arrow, normalColor, 0.06f);

					if (rotationArrow != null) {
						LeanTween.color (rotationArrow, normalColor, 0.06f);
					}
						
				}

				if (typeOfHandle == handleType.Rotation) {
					LeanTween.color (arrowPart1, normalColor, 0.06f);
					LeanTween.color (arrowPart2, normalColor, 0.06f);
					LeanTween.color (arrowPart3, normalColor, 0.06f);
					handles.ShowRotationHandles ();
				}

				if (typeOfHandle == handleType.RotationHandleToggle) {
					LeanTween.moveLocal(bottomPartSphere, standardPosBottom, 0.2f).setEase(LeanTweenType.easeOutQuad);
				}

				if (rotationVisual != null) {
					Destroy (rotationVisual);
					Destroy (CurrentRotationLine);
					Destroy (RotationOnStartLine);
				}

                controller.DeAssignCurrentFocus(transform.gameObject);
				handles.objectFocused = false;
				//LeanTween.color(this.gameObject, Color.white, 0.2f);
				focused = false;
			}
		}
    }

    public void UnLock()
    {
        locked = false;
    }
}
