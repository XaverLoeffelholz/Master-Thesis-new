using UnityEngine;
using System.Collections;

public class handle : MonoBehaviour {

    public enum handleType
    {
        ScaleFace,
        PositionCenter,
        Height,
        RotationX,
        RotationY,
        RotationZ,
        RotationXstepwise,
        RotationYstepwise,
        RotationZstepwise
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

    // Use this for initialization
    void Start () {
        ResetLastPosition();
        connectedModelingObject = connectedObject.GetComponent<ModelingObject>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (rotateStep && !locked)
        {
            RotateStepwise();
        }
    }

    private float CalculateInputFromPoint(Vector3 pointOfCollision)
    {

        if (resetLastPosition)
        {
            directionHandle = Vector3.Normalize(p2.transform.position - p1.transform.position);

            if (face != null)
            {
                initialLocalPositionFace = face.center.coordinates;
                initialDistancceCenterScaler = face.scalerPosition - face.centerPosition;
            }

            initialLocalPositionHandle = transform.localPosition;
            initialPositionHandle = transform.position;
        }

        Vector3 pq = pointOfCollision - p1.transform.position;
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

    public void ApplyChanges(GameObject pointOfCollision)
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
            case handleType.RotationX:
                Rotate(pointOfCollision);
                break;
            case handleType.RotationY:
                Rotate(pointOfCollision);
                break;
            case handleType.RotationZ:
                Rotate(pointOfCollision);
                break;
            case handleType.RotationXstepwise:
                SetRotateStepTrue();
                break;
            case handleType.RotationYstepwise:
                SetRotateStepTrue();
                break;
            case handleType.RotationZstepwise:
                SetRotateStepTrue();
                break;
        }

        connectedObject.GetComponent<ModelingObject>().RecalculateSideCenters();
        connectedObject.GetComponent<ModelingObject>().RecalculateNormals();
    }

    private void ScaleFace(GameObject pointOfCollision)
    {
        float input = CalculateInputFromPoint(pointOfCollision.transform.position);

        Vector3 positionScaler = initialLocalPositionFace + ((1f + input) * initialDistancceCenterScaler);
        Vector3 newDistanceCenterScaler = positionScaler - face.centerPosition;

        if (newDistanceCenterScaler.magnitude >= 0.1f && Vector3.Dot(initialDistancceCenterScaler, newDistanceCenterScaler)>0)
        {
            Vector3 position = initialLocalPositionHandle + (-input * (face.scalerPosition-face.centerPosition).normalized);
            transform.localPosition = RasterManager.Instance.Raster(position);
            //transform.localPosition = position;


            face.scaler.coordinates = RasterManager.Instance.Raster(positionScaler);
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
         float input = CalculateInputFromPoint(pointOfCollision.transform.position);

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
        float input = CalculateInputFromPoint(pointOfCollision.transform.position);

        transform.position = initialPositionHandle + (input * 0.5f * directionHandle);

        input = RasterManager.Instance.RasterAngle(input);
        connectedModelingObject.RotateAround((RotationAxis.transform.position - connectedObject.transform.position), input*0.5f);

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

            if (typeOfHandle == handleType.RotationXstepwise)
            {
                Debug.Log("hovering arrow");
            }

			if (!clicked && !handles.objectFocused)	{

                controller.AssignCurrentFocus(transform.gameObject);
				handles.objectFocused = true;

				LeanTween.color(this.gameObject, Color.cyan, 0.2f);
				focused = true;
			}
		}

    }

    public void UnFocus(Selection controller)
    {
		if (focused) {
			if(!controller.triggerPressed)
			{
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
