using UnityEngine;
using System.Collections;

public class BiManualOperations : Singleton<BiManualOperations> {

    public Selection controller1;
    public Selection controller2;

    private bool scalingStarted;
    private Vector3 initialscale;
    private Vector3 initialDistance;
    private Vector3 lastDistance;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate() {
	    if (controller1.triggerPressed && controller2.triggerPressed && controller1.currentFocus == controller2.currentFocus)
        {
            if (!scalingStarted)
            {
                StartScaling();
            } else
            {
                ScaleObject();
            }

        } else
        {
            scalingStarted = false;
            controller1.scalingObject = false;
            controller2.scalingObject = false;
        }
	}

    public void StartScaling()
    {
        controller1.CreatePointOfCollisionPrefab();
        controller2.CreatePointOfCollisionPrefab();

        controller1.scalingObject = true;
        controller2.scalingObject = true;

        scalingStarted = true;

        initialscale = controller1.currentFocus.transform.localScale;
        controller1.currentFocus.GetComponent<ModelingObject>().StartScaling();

        initialDistance = controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position;
        lastDistance = initialDistance;
    }

    public void ScaleObject()
    {
        Vector3 newDistance = controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position;

        // move scaler of object
        float newScale = newDistance.magnitude / initialDistance.magnitude;

        controller1.currentFocus.GetComponent<ModelingObject>().ScaleBy(newScale);


        // old Rotation:
        /*
        Vector3 rotation = Quaternion.FromToRotation((controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position), lastDistance).eulerAngles;

        rotation = new Vector3(RasterManager.Instance.RasterAngle(rotation.x), RasterManager.Instance.RasterAngle(rotation.y), RasterManager.Instance.RasterAngle(rotation.z));

        // check that they are not 360
        if (!((rotation.x == 0f || rotation.x == 360f) && (rotation.y == 0f || rotation.y == 360f) && (rotation.z == 0f || rotation.z == 360f)))
        {
            lastDistance = newDistance;
        }

        controller1.currentFocus.GetComponent<ModelingObject>().RotateAround(new Vector3(1f, 0f, 0f), -rotation.x);
        controller1.currentFocus.GetComponent<ModelingObject>().RotateAround(new Vector3(0f, 1f, 0f), -rotation.y);
        controller1.currentFocus.GetComponent<ModelingObject>().RotateAround(new Vector3(0f, 0f, 1f), -rotation.z);
        */



        //  new Rotation:
        /*
        Vector3 cross = Vector3.Cross(lastDistance, newDistance);
        float amountToRot = RasterManager.Instance.RasterAngle(Vector3.Angle(lastDistance, newDistance));


        if (amountToRot != 0f && amountToRot != 360f)
        {
           lastDistance = newDistance;
        }

            controller1.currentFocus.GetComponent<ModelingObject>().RotateAround(cross.normalized, amountToRot);

        */


    }

    public bool IsScalingStarted()
    {
        return scalingStarted;
    }
}
