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

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        selection = this.GetComponent<Selection>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)){
            touchDown = true;
            lastX = device.GetAxis().x;
            lastY = device.GetAxis().y;
            UiCanvasGroup.Instance.Hide();
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
                Vector3 ObjectToController = selection.currentFocus.transform.position - transform.position;

                // move object on line when using touchpad
                selection.currentFocus.transform.position = selection.currentFocus.transform.position + ObjectToController * amountY;
                selection.currentFocus.transform.localPosition = RasterManager.Instance.Raster(selection.currentFocus.transform.localPosition);
            }
            else if (scaleMode)
            {
                // turn y value into scale of stage          
                float amountY = device.GetAxis().y - lastY;
                lastY = device.GetAxis().y;

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
}
