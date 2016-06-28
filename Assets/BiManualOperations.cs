using UnityEngine;
using System.Collections;

public class BiManualOperations : Singleton<BiManualOperations> {

    public Selection controller1;
    public Selection controller2;

    private bool scalingStarted;
    private Vector3 initialscale;
    private Vector3 initialDistance;

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

        initialDistance = controller1.pointOfCollisionGO.transform.position - controller2.pointOfCollisionGO.transform.position;
    }

    public void ScaleObject()
    {
        // move scaler of object
        float newScale = (controller1.pointOfCollisionGO.transform.position - controller2.pointOfCollisionGO.transform.position).magnitude / initialDistance.magnitude;

        controller1.currentFocus.GetComponent<ModelingObject>().ScaleBy(newScale);
    }

    public bool IsScalingStarted()
    {
        return scalingStarted;
    }
}
