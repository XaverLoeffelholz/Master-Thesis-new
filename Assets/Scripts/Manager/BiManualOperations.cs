using UnityEngine;
using System.Collections;

public class BiManualOperations : Singleton<BiManualOperations> {

    public enum bimanualMode { scaling, rotating};

    public bimanualMode currentBimanualMode;

    public Selection controller1;
    public Selection controller2;

    private bool scalingStarted;
    private Vector3 initialscale;
    private Vector3 initialDistance;
    private Vector3 lastDistance;
    private Quaternion lastRotation;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (controller1.triggerPressed && controller2.triggerPressed && controller1.currentFocus == controller2.currentFocus && controller1.currentFocus.CompareTag("ModelingObject"))
        {
            if (!scalingStarted)
            {
                StartScalingRotating();
            } else
            {
                ScaleOrRotateObject();
            }

        } else
        {
            scalingStarted = false;
            controller1.scalingObject = false;
            controller2.scalingObject = false;
        }
	}

    public void StartScalingRotating()
    {
		if (controller1.currentFocus != null) {

			if (controller1.currentFocus.CompareTag("ModelingObject")){
				controller1.CreatePointOfCollisionPrefab();
				controller2.CreatePointOfCollisionPrefab();

				controller1.scalingObject = true;
				controller2.scalingObject = true;

				scalingStarted = true;

				initialscale = controller1.currentFocus.transform.localScale;

				ModelingObject modObject = controller1.currentFocus.GetComponent<ModelingObject>();

				modObject.StartScaling(true);

				initialDistance = controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position;
				lastDistance = initialDistance;

                // get current rotation and raster it
                lastRotation = Quaternion.LookRotation(controller1.pointOfCollisionGO.transform.position - controller2.pointOfCollisionGO.transform.position);

                float x = RasterManager.Instance.RasterAngle(lastRotation.eulerAngles.x);
                float y = RasterManager.Instance.RasterAngle(lastRotation.eulerAngles.y);
                float z = RasterManager.Instance.RasterAngle(lastRotation.eulerAngles.z);

                lastRotation = Quaternion.Euler(new Vector3(x, y, z));
            }
		}  
    }

    public void ScaleOrRotateObject()
    {
        if (currentBimanualMode == bimanualMode.scaling)
        {
            Vector3 newDistance = controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position;

            // move scaler of object
            float newScale = newDistance.magnitude / initialDistance.magnitude;

            ModelingObject modObject = controller1.currentFocus.GetComponent<ModelingObject>();

			if (newScale > 5f) {
				newScale = 5f;
			}

			if (newScale < 0f) {
				newScale = 0f;
			}

            modObject.ScaleBy(newScale, true);
        }
        else
        {
            Quaternion newRotation = Quaternion.LookRotation(controller1.pointOfCollisionGO.transform.position - controller2.pointOfCollisionGO.transform.position);

            float x = RasterManager.Instance.RasterAngle(newRotation.eulerAngles.x);
            float y = RasterManager.Instance.RasterAngle(newRotation.eulerAngles.y);
            float z = RasterManager.Instance.RasterAngle(newRotation.eulerAngles.z);

            newRotation = Quaternion.Euler(new Vector3(x, y, z));

            Quaternion relative = Quaternion.Inverse(lastRotation) * newRotation;

            lastRotation = newRotation;

            controller1.currentFocus.GetComponent<ModelingObject>().Rotate(relative);


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

    }

    public bool IsScalingStarted()
    {
        return scalingStarted;
    }
}
