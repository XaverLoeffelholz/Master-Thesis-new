using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

	public enum controllerMode { scalingStage, rotatingStage, pullPushObject, toggleRotateScale };

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

	public bool recognizeNewSwipe;

    void Awake()
    {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        selection = this.GetComponent<Selection>();

		if (selection.typeOfController == Selection.controllerType.mainController)
		{
			scaleMode = true;
		} else
		{
			scaleMode = false;
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
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchDown = false;
        }

        if (touchDown)
        {
			if (currentControllerMode == controllerMode.pullPushObject)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

				if (selection.pointOfCollisionGO != null) {
					// get vector between controller and current object
					Vector3 ObjectToController = selection.pointOfCollisionGO.transform.position - transform.position;

					//maybe move this part into modelingobject
					Vector3 prevPosition = selection.pointOfCollisionGO.transform.position;

					// move object on line when using touchpad
					selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position + ObjectToController * amountY;

					if (selection.currentFocus.CompareTag ("ModelingObject")) {

						selection.pointOfCollisionGO.transform.position = RasterManager.Instance.Raster (selection.pointOfCollisionGO.transform.position);

					}

					// somehow not working
					//selection.AdjustLengthPointer ((selection.pointOfCollisionGO.transform.position - transform.position).magnitude);

				} 


 
            }
			else if (currentControllerMode == controllerMode.scalingStage)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

				ScaleStage (amountY);
            }
			else if (currentControllerMode == controllerMode.rotatingStage)
			{
				// turn x value into rotation of stage
				float amountX = device.GetAxis().x - lastX;
				lastX = device.GetAxis().x;
				stage.Rotate(0, amountX * 25f, 0);
			}
			else if (currentControllerMode == controllerMode.toggleRotateScale)
			{
				if (recognizeNewSwipe) {
					recognizeNewSwipe = false;
					lastX = device.GetAxis().x;
				}
                // turn x value into rotation of stage
                float amountX = device.GetAxis().x - lastX;

	
				if (amountX > 0.6f) {					
					RotateObjectMode();

				} else if(amountX < -0.6f) {					
					ScaleObjectMode();

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

	public void ScaleStage(float amountY){
		
		Vector3 scaleStage = stage.parent.localScale;
		scaleStage = scaleStage + (scaleStage * amountY * 2);

		Vector3 libraryStage = library.localScale;
		libraryStage = libraryStage + (libraryStage * amountY * 2);

		Vector3 trashScale = trash.localScale;
		trashScale = trashScale + (trashScale * amountY * 2);

		if (scaleStage.x >= 0.1f && scaleStage.x <= 4f)
		{
			stage.parent.localScale = scaleStage; 
			library.localScale = libraryStage;
			trash.localScale = trashScale;
		}


	}
}
