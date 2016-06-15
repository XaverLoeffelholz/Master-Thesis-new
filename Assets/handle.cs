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
    };

    public GameObject connectedObject;

    public handleType typeOfHandle;
    public GameObject colliderHandle;
    public handles handles;
    public Face face;
    public Transform p1;
    public Transform p2;
    private bool clicked;
	public bool focused = false;
    private Vector3 lastPosition;
    private bool resetLastPosition = true;


	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
     
    }

    private float CalculateInputFromPoint(GameObject pointOfCollision)
    {
        Vector3 u = p2.transform.position - p1.transform.position;
        Vector3 pq = pointOfCollision.transform.position - p1.transform.position;
  
        Vector3 newPoint = p1.transform.position + (u * (Vector3.Dot(pq, u) / u.sqrMagnitude));

        if (resetLastPosition)
        {
            lastPosition = newPoint;
            resetLastPosition = false;
        }

        float input = (newPoint - lastPosition).magnitude;

        // check direction of vector:
        if ((newPoint - lastPosition).x / u.x < 0 || (newPoint - lastPosition).y / u.y < 0 || (newPoint - lastPosition).z / u.z < 0)
        {
            input = input * (-1f);
        }

        lastPosition = newPoint;

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
                RotateX(pointOfCollision);
                break;
            case handleType.RotationY:
                RotateY(pointOfCollision);
                break;
            case handleType.RotationZ:
                RotateZ(pointOfCollision);
                break;
        }

        connectedObject.GetComponent<ModelingObject>().RecalculateSideCenters();
        connectedObject.GetComponent<ModelingObject>().RecalculateNormals();
    }

    private void ScaleFace(GameObject pointOfCollision)
    {
        float input = CalculateInputFromPoint(pointOfCollision);

        Vector3 position = colliderHandle.transform.localPosition;
        position.x += input * 0.5f;

        colliderHandle.transform.localPosition = position;

        face.scaleFace(input * 0.5f);
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
    }

    private void ChangeHeight(GameObject pointOfCollision)
    {
        // move in direction of normals

        float input = CalculateInputFromPoint(pointOfCollision);
        Vector3 position = transform.localPosition;
        position += input * (-0.2f) * face.normal;

        transform.localPosition = RasterManager.Instance.Raster(position);

        Vector3 positionFace = face.center.coordinates;
        positionFace += input * (-0.2f) * face.normal;
        face.center.coordinates = RasterManager.Instance.Raster(positionFace);

    }

    private void RotateX(GameObject pointOfCollision)
    {
        float input = CalculateInputFromPoint(pointOfCollision);

        // rotate
        Vector3 rotation = new Vector3(0,0,0);


        if (input > 0)
        {
            rotation.x += input * (15f);
        }
       
        connectedObject.transform.Rotate(rotation);
    }

    private void RotateY(GameObject pointOfCollision)
    {
        float input = CalculateInputFromPoint(pointOfCollision);

        // rotate
        Vector3 rotation = new Vector3(0, 0, 0);


        if (input > 0)
        {
            rotation.y += input * (15f);
        }
       

        connectedObject.transform.Rotate(rotation);
    }

    private void RotateZ(GameObject pointOfCollision)
    {
        float input = CalculateInputFromPoint(pointOfCollision);
        
        // rotate
        Vector3 rotation = new Vector3(0, 0, 0);

        if (input > 0)
        {
            rotation.z += input * (-15f);
        }        

        connectedObject.transform.Rotate(rotation);
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
}
