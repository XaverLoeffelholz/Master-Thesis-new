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

	private float smoothTime = 0.2f;
	private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (controller1.currentFocus != null && controller2.currentFocus != null) {
			if (controller1.currentFocus == controller2.currentFocus && controller1.currentFocus.CompareTag ("ModelingObject")) {
				if (controller1.currentFocus.GetComponent<ModelingObject> ().group == null) {
					controller1.duplicateMode = false;
					controller2.duplicateMode = false;

					controller1.scalingMode = true;
					controller2.scalingMode = true;

					if (controller1.triggerPressed && controller2.triggerPressed) {
						if (!scalingStarted) {
							StartScalingRotating ();
						} else {
							ScaleOrRotateObject ();
						}
					} else {
						scalingStarted = false;

						if (controller1.scalingObject) {
							controller1.currentFocus.GetComponent<ModelingObject> ().StartMoving (controller1, controller1.currentFocus.GetComponent<ModelingObject> ());
						}

						controller1.scalingObject = false;
						controller2.scalingObject = false;
					}
				}
			} else {
				controller1.scalingMode = false;
				controller2.scalingMode = false;
			}
		} else {
			controller1.scalingMode = false;
			controller2.scalingMode = false;
		}
	}

    public void StartScalingRotating()
    {
		if (controller1.currentFocus != null) {

			if (controller1.currentFocus.CompareTag("ModelingObject")){
				ModelingObject modObject = controller1.currentFocus.GetComponent<ModelingObject>();

				controller1.StartScaling ();
				controller2.StartScaling ();

				scalingStarted = true;

				initialscale = controller1.currentFocus.transform.localScale;
				modObject.StopMoving (controller1, modObject);
				modObject.StartScaling(true);
				initialDistance = controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position;
				lastDistance = initialDistance;
            }
		}  
    }

    public void ScaleOrRotateObject()
    {
        if (currentBimanualMode == bimanualMode.scaling)
        {
			Vector3 newDistance = Vector3.SmoothDamp (lastDistance, controller2.pointOfCollisionGO.transform.position - controller1.pointOfCollisionGO.transform.position, ref velocity, smoothTime);

            // move scaler of object
            float newScale = newDistance.magnitude / initialDistance.magnitude;

            ModelingObject modObject = controller1.currentFocus.GetComponent<ModelingObject>();

			if (newScale > 3f) {
				newScale = 3f;
			}

			if (newScale < 0.1f) {
				newScale = 0.1f;
			}

			lastDistance = newDistance;

            modObject.ScaleBy(newScale, true);
        }
    }

    public bool IsScalingStarted()
    {
        return scalingStarted;
    }
}
