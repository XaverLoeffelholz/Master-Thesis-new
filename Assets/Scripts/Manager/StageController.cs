using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

	public enum controllerMode { scalingStage, rotatingStage, pullPushObject, toggleRotateScale, rotatingObject };

	public controllerMode standardControllerMode;
	public controllerMode currentControllerMode;

    public Transform stage;
    public Transform library;
    public Transform trash;
    private float lastY;
    private float lastX;
    private bool touchDown;
    SteamVR_TrackedObject trackedObj;

    [HideInInspector]
    public bool scaleMode;
    private Selection selection;

	public GameObject pullIcon;
	public GameObject normalIcon;
	public GameObject toggle;
	public GameObject toggle2;
	public GameObject toggleBG;

	public GameObject line;
	private Vector3 lineInitialScale;
	public Color lineColorNormal;
	public Color lineColorTouch;

	public bool recognizeNewSwipe;

	public Vector3 minPosLine;
	public Vector3 maxPosLine;

	public GameObject touchpad;
	private Vector3 touchpadInitialScale;
	private Vector3 touchpadInitialPos;

	private float amountOfMovementForHapticFeedback;

    void Awake()
    {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        selection = this.GetComponent<Selection>();

		lineInitialScale = line.transform.localScale;
		touchpadInitialScale = touchpad.transform.localScale;
		touchpadInitialPos = touchpad.transform.localPosition;

		if (selection.typeOfController == Selection.controllerType.mainController)
		{
			scaleMode = true;

			// move line to point
			float posLine = (stage.parent.localScale.x - 0.25f) / 3.75f;
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
		} else
		{
			scaleMode = false;

			// move line to point for rotation
			// move line to point
			float posLine = ((stage.localRotation.eulerAngles.y - (Mathf.Floor(stage.localRotation.eulerAngles.y/90f) * 90f)) / 90f);
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
		}


    }


	public void ShowPullVisual(bool value){		
		if (value) {
			currentControllerMode = controllerMode.pullPushObject;
			pullIcon.SetActive (true);
			normalIcon.SetActive (false);
			toggle.SetActive (false);
			toggle2.SetActive (false);
			toggleBG.SetActive (false);
		} else {
			currentControllerMode = standardControllerMode;
			pullIcon.SetActive (false);
			normalIcon.SetActive (true);
			toggle.SetActive (false);
			toggle2.SetActive (false);
			toggleBG.SetActive (false);
		}
	}

	public void ShowRotateObjectVisual(bool value){		
		if (value) {
			currentControllerMode = controllerMode.rotatingObject;
			pullIcon.SetActive (false);
			normalIcon.SetActive (true);
			toggle.SetActive (false);
			toggle2.SetActive (false);
			toggleBG.SetActive (false);
		} else {
			currentControllerMode = standardControllerMode;
			pullIcon.SetActive (false);
			normalIcon.SetActive (true);
			toggle.SetActive (false);
			toggle2.SetActive (false);
			toggleBG.SetActive (false);
		}
	}


	public void ShowScaleRotationToggle(bool value) {
		
		if (value) {
			currentControllerMode = controllerMode.toggleRotateScale;
			toggleBG.SetActive (true);

			recognizeNewSwipe = true;

			if (BiManualOperations.Instance.currentBimanualMode == BiManualOperations.bimanualMode.scaling) {
				toggle.SetActive (true);
				toggle2.SetActive (false);
			} else {
				toggle.SetActive (false);
				toggle2.SetActive (true);
			}

			pullIcon.SetActive (false);
			normalIcon.SetActive (false);

		} else {
			currentControllerMode = standardControllerMode;
			toggle.SetActive (false);
			toggle2.SetActive (false);
			pullIcon.SetActive (false);
			normalIcon.SetActive (true);
			toggleBG.SetActive (false);
		}
	}


    // Update is called once per frame
    void FixedUpdate()
    {
		
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)){
            touchDown = true;
            lastX = device.GetAxis().x;
            lastY = device.GetAxis().y;
			amountOfMovementForHapticFeedback = 0f;

			LeanTween.color (line, lineColorTouch, 0.1f);
			LeanTween.scale (touchpad, touchpadInitialScale * 1.2f, 0.2f); 
			LeanTween.moveLocalY (touchpad, touchpadInitialPos.y + 0.003f, 0.2f);

			if (currentControllerMode != controllerMode.pullPushObject) {
				device.TriggerHapticPulse (2000);
			}
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchDown = false;

			LeanTween.color (line, lineColorNormal, 0.1f);
			LeanTween.scale (touchpad, touchpadInitialScale, 0.2f).setDelay(0.1f); 
			LeanTween.moveLocalY (touchpad, touchpadInitialPos.y, 0.2f).setDelay(0.1f);

			device.TriggerHapticPulse (1400);
        }

        if (touchDown)
        {
			if (currentControllerMode == controllerMode.pullPushObject)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

				amountOfMovementForHapticFeedback += amountY;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}

				if (selection.pointOfCollisionGO != null) {
					// get vector between controller and current object
					Vector3 ObjectToController = selection.pointOfCollisionGO.transform.position - transform.position;

					// move object on line when using touchpad
					selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position + ObjectToController * amountY*1.4f;

					if (selection.currentFocus.CompareTag ("ModelingObject")) {
						selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position;
					}

					float distance = (selection.pointOfCollisionGO.transform.position - transform.position).magnitude;

					// somehow not working
					selection.AdjustLengthPointer (distance);

					// move line to point
					float posLine = ((distance - (Mathf.Floor(distance/5f) * 5f)) / 5f);
					line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

					float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
					line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
				} 


 
            }
			else if (currentControllerMode == controllerMode.scalingStage)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

				amountOfMovementForHapticFeedback += amountY;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}


				ScaleStage (amountY);
            }
			else if (currentControllerMode == controllerMode.rotatingStage)
			{
				// turn x value into rotation of stage
				float amountX = device.GetAxis().x - lastX;
				lastX = device.GetAxis().x;

				amountOfMovementForHapticFeedback += amountX;

				if (amountOfMovementForHapticFeedback > 2f) {
					device.TriggerHapticPulse (1000);
					amountOfMovementForHapticFeedback = 0f;
				}


				RotateStage (amountX);
			}
			else if (currentControllerMode == controllerMode.rotatingObject)
			{
				if (selection.otherController.currentFocus.CompareTag("ModelingObject")){
					
					// turn x value into rotation of stage
					float amountX = device.GetAxis().x - lastX;
					lastX = device.GetAxis().x;

					amountOfMovementForHapticFeedback += amountX;

					if (amountOfMovementForHapticFeedback > 2f) {
						device.TriggerHapticPulse (1000);
						amountOfMovementForHapticFeedback = 0f;
					}

					ModelingObject currentModelingObject = selection.otherController.currentFocus.GetComponent<ModelingObject> ();

					currentModelingObject.RotateAround (currentModelingObject.GetBoundingBoxTopCenter () - currentModelingObject.GetBoundingBoxTopCenter (), amountX * 150f); 
				}
			}

        }

    }

    public void DoClick(object sender, ClickedEventArgs e)
    {

    }

	public void ScaleObjectMode(){
		BiManualOperations.Instance.currentBimanualMode = BiManualOperations.bimanualMode.scaling;
		recognizeNewSwipe = true;

		toggle.SetActive (true);
		toggle2.SetActive (false);
	}

	public void RotateObjectMode(){
		
		BiManualOperations.Instance.currentBimanualMode = BiManualOperations.bimanualMode.rotating;
		recognizeNewSwipe = true;

		toggle.SetActive (false);
		toggle2.SetActive (true);
	}



	public void ToggleMode(){

	}

	public void RotateStage(float amountX){
		stage.Rotate(0, amountX * 25f, 0);

		// move line to point
		float posLine = ((stage.localRotation.eulerAngles.y - (Mathf.Floor(stage.localRotation.eulerAngles.y/90f) * 90f)) / 90f);
		line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

		float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
		line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x*0.7f), lineInitialScale.y, lineInitialScale.z);
	}

	public void ScaleStage(float amountY){
		
		Vector3 scaleStage = stage.parent.localScale;
		scaleStage = scaleStage + (scaleStage * amountY);

	//	Vector3 libraryStage = library.localScale;
	//	libraryStage = libraryStage + (libraryStage * amountY);

		if (scaleStage.x >= 0.25f && scaleStage.x <= 4f)
		{
			stage.parent.localScale = scaleStage; 
		//	library.localScale = libraryStage;

			// move line to point
			float posLine = (scaleStage.x - 0.25f) / 3.75f;
			line.transform.localPosition = minPosLine + posLine * (maxPosLine - minPosLine);

			float scale = Mathf.Sqrt((Mathf.Abs(posLine - 0.5f)) / 0.5f);
			line.transform.localScale = new Vector3 (Mathf.Max(lineInitialScale.x * (1f-scale), lineInitialScale.x * 0.7f), lineInitialScale.y, lineInitialScale.z);
		}
	}
}
