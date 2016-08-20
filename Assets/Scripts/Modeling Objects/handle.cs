using UnityEngine;
using System.Collections;

public class handle : MonoBehaviour {

    public enum handleType
    {
        ScaleFace,
        PositionCenter,
        Height,
        Rotation,
        RotationX,
        RotationY,
        RotationZ
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

    // Use this for initialization
    void Start () {
        ResetLastPosition();
        connectedModelingObject = connectedObject.GetComponent<ModelingObject>();

		if (arrow != null) {
			initialSizeArrow = arrow.transform.localScale;
		}
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (rotateStep && !locked)
        {
            RotateStepwise();
        }
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

			/*
			if (circle != null) {
				initialScaleCircle = circle.localScale;
			}*/
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
                    newRotation = true;
                }                
                Rotate(pointOfCollision);
                break;
        }

		connectedModelingObject.RecalculateSideCenters();
		connectedModelingObject.RecalculateNormals();

		connectedModelingObject.ShowBoundingBox ();
    }

	public void FinishUsingHandle(){
		if (typeOfHandle == handleType.Rotation) {
			handles.ShowRotationHandles();
		}
	}

    private void ScaleFace(GameObject pointOfCollision)
    {
		float input = CalculateInputFromPoint(pointOfCollision.transform.position, p1.transform.position, p2.transform.position)*1.5f;

        Vector3 positionScaler = initialLocalPositionFace + ((1f - input) * initialDistancceCenterScaler);
        Vector3 newDistanceCenterScaler = positionScaler - face.centerPosition;

        if (newDistanceCenterScaler.magnitude >= 0.1f && Vector3.Dot(initialDistancceCenterScaler, newDistanceCenterScaler)>0)
        {
			//Vector3 position = initialLocalPositionHandle + (-input * (initialLocalPositionHandle-face.centerPosition).normalized * 0.5f);

			positionScaler = RasterManager.Instance.Raster(positionScaler);
			transform.position = connectedModelingObject.transform.TransformPoint(positionScaler);
			face.scaler.coordinates = positionScaler;

            // update scale of circle
			if (circle != null) {
				//circle.localScale = initialScaleCircle * (newDistanceCenterScaler.magnitude / initialDistancceCenterScaler.magnitude); 
				//circle.localScale = new Vector3(newDistanceCenterScaler.magnitude,newDistanceCenterScaler.magnitude,newDistanceCenterScaler.magnitude);

			}

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

         Vector3 position = initialLocalPositionHandle + ((-input * 1.4f) * face.normal);
         Vector3 positionFace = initialLocalPositionFace + ((-input * 1.4f) * face.normal);
    
        // check that center does not get below other center

        if ((face.typeOfFace == Face.faceType.TopFace && positionFace.y > face.parentModelingObject.bottomFace.centerPosition.y) || (face.typeOfFace == Face.faceType.BottomFace && positionFace.y < face.parentModelingObject.topFace.centerPosition.y))
        {
            transform.localPosition = RasterManager.Instance.Raster(position);
           // transform.localPosition = position;

            face.center.coordinates = RasterManager.Instance.Raster(positionFace);
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
			
        // define rotation axis
		Vector3 rotationAxis = connectedModelingObject.transform.InverseTransformDirection(p1.transform.position - p2.transform.position);

        // rotate around this axis
		connectedModelingObject.RotateAround(rotationAxis, RasterManager.Instance.RasterAngle(newRotationAmount-prevRotationAmount));

		prevRotationAmount =  RasterManager.Instance.RasterAngle(newRotationAmount);
    }

    private void SetRotateStepTrue()
    {
        rotateStep = true;
    }

    private void RotateStepwise()
    {
        connectedModelingObject.RotateAround((RotationAxis.transform.position - connectedObject.transform.position), 45f);
        rotateStep = false;
        locked = true;
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
					LeanTween.color (arrow, new Color (0.7f, 0.8f, 1f, 1f), 0.1f);
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
					LeanTween.color(arrow, new Color(1f,1f,1f,1f), 0.1f);
				}

                controller.DeAssignCurrentFocus(transform.gameObject);
				handles.objectFocused = false;
				LeanTween.color(this.gameObject, Color.white, 0.2f);
				focused = false;
			}
		}
    }

    public void UnLock()
    {
        locked = false;
    }
}
