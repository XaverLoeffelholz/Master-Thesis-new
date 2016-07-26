using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

    public Transform stage;
    public Transform library;
    public Transform trash;
    private float lastY;
    private float lastX;
    private bool touchDown;
    SteamVR_TrackedObject trackedObj;
    public bool scaleMode = false;
    private Selection selection;

	public GameObject pullIcon;
	public GameObject normalIcon;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        selection = this.GetComponent<Selection>();
    }


	public void ShowPullVisual(bool value){
		if (value) {
			pullIcon.SetActive (true);
			normalIcon.SetActive (false);
		} else {
			pullIcon.SetActive (false);
			normalIcon.SetActive (true);
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

			/*
            UiCanvasGroup.Instance.Hide();

			if (selection.currentSelection != null && selection.currentSelection.CompareTag("ModelingObject")) {
				selection.currentSelection.GetComponent<ModelingObject> ().DeSelect (selection);
			}

			if (selection.otherController.currentSelection != null) {
				selection.otherController.currentSelection.GetComponent<ModelingObject> ().DeSelect (selection.otherController);
			}
			*/
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchDown = false;
        }

        if (touchDown)
        {
            if (selection.GetObjectMoving())
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

                // get vector between controller and current object
				Vector3 ObjectToController = selection.pointOfCollisionGO.transform.position - transform.position;

                //maybe move this part into modelingobject
				Vector3 prevPosition = selection.pointOfCollisionGO.transform.position;

                // move object on line when using touchpad
				selection.pointOfCollisionGO.transform.position = selection.pointOfCollisionGO.transform.position + ObjectToController * amountY;

				if (selection.currentFocus.CompareTag ("ModelingObject")) {
					
					selection.pointOfCollisionGO.transform.position = RasterManager.Instance.Raster(selection.pointOfCollisionGO.transform.position);

					/*
					Group objectgroup = selection.currentFocus.GetComponent<ModelingObject>().group;

					// if object is grouped, add movement to group
					if (objectgroup != null)
					{
						objectgroup.Move(selection.pointOfCollisionGO.transform.position - prevPosition, selection.currentFocus.GetComponent<ModelingObject>());
					}*/
				}

				// somehow not working
				//selection.AdjustLengthPointer ((selection.pointOfCollisionGO.transform.position - transform.position).magnitude);
 
            }
            else if (scaleMode)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

				ScaleStage (amountY);
            }
            else
            {
                // turn x value into rotation of stage
                float amountX = device.GetAxis().x - lastX;
                lastX = device.GetAxis().x;
                stage.Rotate(0, amountX * 25f, 0);
            }

        }

    }

    public void DoClick(object sender, ClickedEventArgs e)
    {

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
