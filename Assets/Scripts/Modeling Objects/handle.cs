using UnityEngine;
using System.Collections;

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
		ScaleMinusZ
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
	private Vector3 initialSizeArrow;
    private bool rotating = false;
    private bool newRotation = false;
    private float prevRotationAmount;

	private bool newScaling = false;
	private float prevScalingAmount;

	public Color normalColor;
	public Color hoverColor;

	private Vector3 centerOfScaling = new Vector3(0f,0f,0f);
	private Vector3 touchPointForScaling = new Vector3(0f,0f,0f);

	public Vector3 boundingBoxCorner1;
	public Vector3 boundingBoxCorner2;

    // Use this for initialization
    void Start () {
		if (arrow != null) {
			arrow.GetComponent<Renderer>().material.color = normalColor;
		}


        ResetLastPosition();
        connectedModelingObject = connectedObject.GetComponent<ModelingObject>();

		if (arrow != null) {
			initialSizeArrow = arrow.transform.localScale;
		}
    }
	
	// Update is called once per frame
	void FixedUpdate () {

    }

	private float CalculateInputFromPoint(Vector3 pointOfCollision, Vector3 pos1, Vector3 pos2)
    {

        if (resetLastPosition)
        {
			directionHandle = Vector3.Normalize(pos2 - pos1);

            if (face != null)
            {
                initialLocalPositionFace = face.center.coordinates;
                initialDistancceCenterScaler = face.scalerPosition - face.centerPosition;
            }

            initialLocalPositionHandle = transform.localPosition;
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

    public void ApplyChanges(GameObject pointOfCollision, bool alreadyMoving)
    {
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
                if (!alreadyMoving)
                {
					handles.HideRotationHandlesExcept (this);
					handles.HideScalingHandlesExcept (null);
                    newRotation = true;
                }                
                Rotate(pointOfCollision);
                break;
			case handleType.ScaleX:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}         
				ScaleNonUniform (pointOfCollision, new Vector3 (1f, 0f, 0f));
				break;
			case handleType.ScaleY:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3 (0f, 1f, 0f));
				break;
			case handleType.ScaleZ:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3 (0f, 0f, 1f));
				break;
			case handleType.ScaleMinusX:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3 (1f, 0f, 0f));
				break;
			case handleType.ScaleMinusY:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3 (0f, 1f, 0f));
				break;
			case handleType.ScaleMinusZ:
				if (!alreadyMoving)
				{
					handles.HideRotationHandlesExcept (null);
					handles.HideScalingHandlesExcept (this);
					newScaling = true;
				}  
				ScaleNonUniform (pointOfCollision, new Vector3 (0f, 0f, 1f));
				break;
        }

		connectedModelingObject.RecalculateSideCenters();
		connectedModelingObject.RecalculateNormals();

		handles.HideRotationHandlesExcept (this);
		handles.HideScalingHandlesExcept (this);

		//connectedModelingObject.ShowBoundingBox ();
		//connectedModelingObject.ShowBoundingBox ();
    }

	public void ScaleNonUniform(GameObject pointOfCollision, Vector3 direction){
		
		Vector3 normalizedDirection = direction.normalized;
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, p1.transform.position, p2.transform.position);

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

		float RasteredDistanceCenters = RasterManager.Instance.Raster((centerOfScaling - touchPointForScaling).magnitude * Mathf.Abs(1f + (input - prevScalingAmount)));
		input = (RasteredDistanceCenters / ((centerOfScaling - touchPointForScaling).magnitude)) - 1f + prevScalingAmount;
			
		// apply rastering!!!
		connectedModelingObject.ScaleNonUniform (Mathf.Abs(1f + (input - prevScalingAmount)), direction, typeOfHandle, centerOfScaling);

		prevScalingAmount = input;
	}

	public void FinishUsingHandle(){
		if (typeOfHandle != handleType.Height && typeOfHandle != handleType.ScaleFace) {
			// handles.ShowRotationHandles();
			handles.ShowNonUniformScalingHandles();
		}
	}

    private void ScaleFace(GameObject pointOfCollision)
    {
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, p1.transform.position, p2.transform.position) * 1.5f;

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
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, p1.transform.position, p2.transform.position);

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

    private void Rotate(GameObject pointOfCollision)
    {
        // get vector from handle to center of object
        Vector3 HandleToCenter = transform.position - connectedModelingObject.transform.position;

        // get vector form direction of handle
		Vector3 handleDirection = p1.transform.position - p2.transform.position;

        // cross product
        Vector3 crossProduct = Vector3.Cross(HandleToCenter, handleDirection) * (-1f);

		float newRotationAmount = 90f*CalculateInputFromPoint(pointOfCollision.transform.position, transform.position, transform.position + crossProduct);

        if (newRotation)
        {
			prevRotationAmount =  RasterManager.Instance.RasterAngle(newRotationAmount);
            newRotation = false;
        }
			

		// here we need to check if it is connected to a group or a modeling object

        // define rotation axis
		Vector3 p1Rotation = connectedModelingObject.GetBoundingBoxCenter();
		Vector3 p2Rotation = p1Rotation + (p1.transform.position - p2.transform.position);

		Vector3 rotationAxis = connectedModelingObject.transform.InverseTransformDirection(p2Rotation - p1Rotation);

        // rotate around this axis
		Vector3 bbCenterBeforeRotation = connectedModelingObject.transform.InverseTransformPoint (connectedModelingObject.GetBoundingBoxCenter ());

		connectedModelingObject.RotateAround(rotationAxis, RasterManager.Instance.RasterAngle(newRotationAmount-prevRotationAmount), bbCenterBeforeRotation);

		prevRotationAmount = RasterManager.Instance.RasterAngle(newRotationAmount);

		connectedModelingObject.CalculateBoundingBox ();
		connectedModelingObject.boundingBox.DrawBoundingBox ();
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
		if (!focused) {

			if (!clicked && !handles.objectFocused)	{
				
				if (arrow != null) {
					// Hover effect: Scale bigger & change color
					LeanTween.scale (arrow, new Vector3 (initialSizeArrow.x * 1.4f, initialSizeArrow.y * 1.4f, initialSizeArrow.z * 1.4f), 0.1f);
					LeanTween.color (arrow, hoverColor, 0.1f);
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
					LeanTween.scale(arrow, new Vector3 (initialSizeArrow.x, initialSizeArrow.y, initialSizeArrow.z), 0.1f);
					LeanTween.color(arrow, normalColor, 0.1f);
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
