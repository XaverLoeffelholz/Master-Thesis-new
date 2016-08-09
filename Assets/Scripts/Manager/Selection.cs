using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Selection : MonoBehaviour
{
    public enum controllerType { mainController, SecondaryController}

    public controllerType typeOfController;

    public GameObject currentFocus;
    public GameObject currentSelection;
	public Vector3 pointOfCollision;
	public Face collidingFace;
    public GameObject objects;
    public Selection otherController;

    public bool triggerPressed = false;
    [HideInInspector]
    public bool scalingObject = false;
    private bool movingObject = false;
    private bool movingHandle = false;

    public GameObject LaserPointer;
    public GameObject pointOfCollisionPrefab;
    private float temps;
	private float tempsUI;
	private bool faceSelection;

    [HideInInspector]
    public GameObject pointOfCollisionGO;

    SteamVR_TrackedObject trackedObj;

    private Vector3 uiPositon;
    public bool groupItemSelection = false;

    public GameObject cam;

	public bool controllerActive;

	public CircleAnimaton circleAnimation;
	public Infopanel trashInfopanel;

	public GameObject grabIconPrefab;
	private GameObject grabIcon;
	private Vector3 initialScaleGrabIcon;

	public GameObject grabbedIconPrefab;
	private GameObject grabbedIcon;
	private Vector3 grabbedIconOffset;
	private Vector3 initialScaleGrabbedIcon;

	private Transform stageScaler;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

		if (typeOfController == controllerType.SecondaryController) {
			ActivateController (false);
		} else {
			ActivateController (true);
		}

		tempsUI = Time.time;

		stageScaler = GameObject.Find ("StageScaler").transform;

		initialScaleGrabIcon = new Vector3(0.005f,0.005f,0.005f);
		initialScaleGrabbedIcon = new Vector3(0.005f,0.005f,0.005f);
    }


	void DeFocusCurrent(GameObject newObject){

		if (grabIcon != null) {
			grabIcon.SetActive(false);
		}

		if (currentFocus != null) {
			if (currentFocus.CompareTag ("ModelingObject")) {
				// compare selection button and object
				if (newObject.CompareTag ("SelectionButton")) {
					if (newObject.GetComponent<ObjectSelecter> ().connectedObject != currentFocus.GetComponent<ModelingObject> ()) {
						currentFocus.GetComponent<ModelingObject> ().UnFocus (this);
					}
				} else {
					currentFocus.GetComponent<ModelingObject> ().UnFocus (this);
				}
			} else if (currentFocus.CompareTag ("Handle")) {
				currentFocus.GetComponent<handle> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("UiElement")) {
				currentFocus.GetComponent<UiElement> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("TeleportPosition")) {
				currentFocus.GetComponent<TeleportationPosition> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("Library")) {
				library.Instance.UnFocus (this);
			} else if (currentFocus.CompareTag ("HeightControl")) {
				currentFocus.GetComponent<StageHeightController> ().UnFocus (this);
			}else if (currentFocus.CompareTag ("InfoPanel")) {
				currentFocus.GetComponent<Infopanel> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("SelectionButton")) {				
				// compare selection button and object
				if (newObject.CompareTag ("ModelingObject")) {
					if (currentFocus.GetComponent<ObjectSelecter> ().connectedObject != newObject.GetComponent<ModelingObject> ()) {
						currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
						currentFocus.GetComponent<ObjectSelecter> ().connectedObject.UnFocus (this);
					} else {
						currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
					}
				} else {
					currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
					currentFocus.GetComponent<ObjectSelecter> ().connectedObject.UnFocus (this);
				} 
			}

			currentFocus = null;
		}

	}

    public void AdjustLengthPointer(float MaxDistance)
    {
        LaserPointer.transform.localScale = new Vector3 (1,1,Mathf.Clamp(MaxDistance, 0f,50f));

        // add visual for contact
    }
	
    public void TriggerIfPressed(ushort duration)
    {
        if (triggerPressed)
        {
            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(duration);
        }
    }

	// Update is called once per frame
    // maybe also try update?
	void FixedUpdate () {

        var device = SteamVR_Controller.Input((int)trackedObj.index);       

		if (controllerActive) {

			if (movingObject && currentFocus != null) {
				if (grabbedIcon != null && grabbedIcon.activeSelf) {
					grabbedIcon.transform.position = currentFocus.transform.position + grabbedIconOffset;
					Vector3 newScale = initialScaleGrabbedIcon * (transform.position-currentFocus.transform.position).magnitude;
					grabbedIcon.transform.localScale = new Vector3(Mathf.Min(newScale.x, grabbedIcon.transform.localScale.x*1.3f), Mathf.Min(newScale.y, grabbedIcon.transform.localScale.y*1.3f), Mathf.Min(newScale.z, grabbedIcon.transform.localScale.z*1.3f)); 
				}
			}

			RaycastHit hit;

			// only change focus is the object is not moved at the moment
			if (!movingObject && !movingHandle && !scalingObject && !triggerPressed) {
				if (Physics.Raycast (LaserPointer.transform.position, LaserPointer.transform.forward, out hit)) {					
					if (currentFocus != null) {
						Vector3 directionLaserPointerObject = (currentFocus.transform.position - LaserPointer.transform.position).normalized;
						uiPositon = LaserPointer.transform.position + directionLaserPointerObject * 0.5f + Vector3.down * 0.27f;
					}

					AdjustLengthPointer (hit.distance);

					if (grabIcon != null && grabIcon.activeSelf) {
						Vector3 newScale = initialScaleGrabIcon * hit.distance;
						grabIcon.transform.localScale = new Vector3(Mathf.Min(newScale.x, grabIcon.transform.localScale.x*1.3f), Mathf.Min(newScale.y, grabIcon.transform.localScale.y*1.3f), Mathf.Min(newScale.z, grabIcon.transform.localScale.z*1.3f)); 
					}

					if (hit.rigidbody != null && hit.rigidbody.transform.parent != null) {

						if (grabIcon == null) {
							grabIcon = Instantiate (grabIconPrefab);
						}


						if (currentFocus != hit.rigidbody.transform.parent.gameObject) {
							// focus of Object
							if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("ModelingObject")) {								
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<ModelingObject> ().Focus (this);
								device.TriggerHapticPulse (300);
								grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
								grabIcon.SetActive (true);
							} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("Handle")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<handle> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("UiElement")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<UiElement> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("TeleportTrigger")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<TeleportationTrigger> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("SelectionButton")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<ObjectSelecter> ().Focus (this);
								currentFocus.GetComponent<ObjectSelecter> ().connectedObject.Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("TeleportPosition")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<TeleportationPosition> ().Focus (this);
								device.TriggerHapticPulse (600);
							} else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("Library")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								library.Instance.Focus (this);
								device.TriggerHapticPulse (600);
								grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
								grabIcon.SetActive (true);
							} else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("HeightControl")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<StageHeightController> ().Focus (this);
								device.TriggerHapticPulse (600);

								grabIcon.transform.localScale = initialScaleGrabIcon * hit.distance; 
								grabIcon.SetActive (true);
							}
							else if (!UiCanvasGroup.Instance.visible && hit.rigidbody.transform.parent.gameObject.CompareTag ("InfoPanel")) {
								DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
								currentFocus = hit.rigidbody.transform.parent.gameObject;
								currentFocus.GetComponent<Infopanel> ().Focus (this);
								device.TriggerHapticPulse (600);
							}
								

						}

						// Set position of collision
						pointOfCollision = hit.point;

						if (grabIcon != null && grabIcon.activeSelf) {
							grabIcon.transform.position = pointOfCollision;
						}

						if (faceSelection) {
							// highlight face that is selected
							if (collidingFace != null) {
								collidingFace.UnHighlight ();
							}

							FindCollidingFace (pointOfCollision);

							if (collidingFace != null) {
								collidingFace.Highlight ();
							}

						}

					} else {
						if (currentFocus != null) {
							DeFocusCurrent (currentFocus.gameObject);
							temps = Time.time;
							AdjustLengthPointer (50f);
						}
					}
				} else {
					if (currentFocus != null) {
						DeFocusCurrent (currentFocus.gameObject);
						temps = Time.time;
					}

					AdjustLengthPointer (50f);
				}
			} 


			if (currentFocus != null)
			{
				if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
				{
					triggerPressed = true;
					temps = Time.time;
				}

				if (triggerPressed && temps > 0.1f 
					&& (currentFocus.CompareTag("ModelingObject") || currentFocus.CompareTag("TeleportPosition") || currentFocus.CompareTag("Library") || currentFocus.CompareTag("HeightControl") )
					&& typeOfController == controllerType.mainController)
				{
					if (!movingObject && !faceSelection && !groupItemSelection){

						if (grabbedIcon == null) {
							grabbedIcon = Instantiate (grabbedIconPrefab);
						}

						grabbedIcon.SetActive (true);

						grabbedIconOffset = grabIcon.transform.position - currentFocus.transform.position; 
						grabbedIcon.transform.position = currentFocus.transform.position + grabbedIconOffset;

						grabIcon.SetActive (false);

						CreatePointOfCollisionPrefab();
						movingObject = true;
						UiCanvasGroup.Instance.Hide();

						if (currentFocus.CompareTag ("ModelingObject")) {

							if (circleAnimation != null) {
								circleAnimation.StartAnimation ();
							}

							this.GetComponent<StageController> ().ShowPullVisual (true);
							//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (true);	
							currentFocus.GetComponent<ModelingObject>().StartMoving(this, currentFocus.GetComponent<ModelingObject>());
							device.TriggerHapticPulse (1800);

							otherController.ActivateController(true);
						} else if (currentFocus.CompareTag ("TeleportPosition")) {
							if (circleAnimation != null) {
								circleAnimation.StartAnimation ();
							}
							this.GetComponent<StageController> ().ShowPullVisual (true);
							//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (true);
							currentFocus.GetComponent<TeleportationPosition>().StartMoving(this);
						} else if (currentFocus.CompareTag ("Library")) {
							if (circleAnimation != null) {
								circleAnimation.StartAnimation ();
							}
							this.GetComponent<StageController> ().ShowPullVisual (true);
							//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (true);
							library.Instance.StartMoving(this);
						} else if (currentFocus.CompareTag ("HeightControl")) {
							currentFocus.GetComponent<StageHeightController>().StartMoving(this);
						}
					}


				} 
				else if (triggerPressed && (movingHandle || currentFocus.CompareTag("Handle")))
				{
					if (pointOfCollisionGO == null)
					{
						CreatePointOfCollisionPrefab();
					}
					currentFocus.GetComponent<handle>().ApplyChanges(pointOfCollisionGO, movingHandle);
					//currentFocus.GetComponent<handle> ().connectedObject.GetComponent<ModelingObject> ().HideBoundingBox ();
					movingHandle = true;

				}
			}

			if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
			{
				triggerPressed = false;
				movingHandle = false;

				if (movingObject && typeOfController == controllerType.mainController)
				{
					otherController.triggerPressed = false;
					otherController.movingHandle = false;

					if (grabbedIcon != null) {
						grabbedIcon.SetActive (false);
					}

					if (currentFocus.CompareTag ("ModelingObject")) {	
						if (circleAnimation != null) {
							circleAnimation.StartAnimation ();
						}

						device.TriggerHapticPulse (1800);
						currentFocus.GetComponent<ModelingObject>().StopMoving(this, currentFocus.GetComponent<ModelingObject>());
						otherController.ActivateController (false);

						this.GetComponent<StageController> ().ShowPullVisual (false);
						//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (false);

						if (currentFocus.GetComponent<ModelingObject>().inTrashArea)
						{
							currentFocus.GetComponent<ModelingObject>().TrashObject(true);
							trashInfopanel.CloseInfoPanel ();

							//currentFocus = null;
							device.TriggerHapticPulse(1000);
						}

						currentFocus.GetComponent<ModelingObject> ().DeSelect (this);
					} else if (currentFocus.CompareTag ("TeleportPosition")) {
						currentFocus.GetComponent<TeleportationPosition>().StopMoving(this);

						this.GetComponent<StageController> ().ShowPullVisual (false);
						//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (false);

						Teleportation.Instance.JumpToPos(5);
					} else if (currentFocus.CompareTag ("Library")) {
						this.GetComponent<StageController> ().ShowPullVisual (false);
						//otherController.transform.GetComponent<StageController> ().ShowScaleRotationToggle (false);

						library.Instance.StopMoving(this);
					} else if (currentFocus.CompareTag ("HeightControl")) {
						currentFocus.GetComponent<StageHeightController>().StopMoving(this);
					}


				}

				Destroy(pointOfCollisionGO);
				movingObject = false;

				if (currentFocus != null)
				{
					if (currentFocus.CompareTag("ModelingObject"))
					{
						if (!groupItemSelection && currentSelection != null && currentSelection != currentFocus)
						{
							UiCanvasGroup.Instance.CloseMenu(this);
							this.enableFaceSelection(false);
						}

						if (groupItemSelection)
						{
							ObjectsManager.Instance.AddObjectToGroup(ObjectsManager.Instance.currentGroup, currentFocus.GetComponent<ModelingObject>());  
							currentFocus.GetComponent<ModelingObject> ().ShowOutline (true);

						} else if (faceSelection) {
							if (collidingFace != null)
							{
								this.enableFaceSelection(false);
								otherController.enableFaceSelection(false);
								collidingFace.CreateNewModelingObject();
								SelectLatestObject();
								UiCanvasGroup.Instance.shapeMenu.ActivateMenu();
								collidingFace = null;
							}
						}
					}
					else if (currentFocus.CompareTag("SelectionButton"))
					{
						UiCanvasGroup.Instance.Show();
						currentFocus.GetComponent<ObjectSelecter>().Select(this, uiPositon);
					}
					else if (currentFocus.CompareTag("Handle"))
					{
						handle currentHandle = currentFocus.GetComponent<handle> ();
						
						currentHandle.ResetLastPosition();
						currentHandle.UnLock();
						currentHandle.UnFocus(this);
						currentHandle.connectedObject.GetComponent<ModelingObject> ().ShowBoundingBox ();
					} 
					else if (currentFocus.CompareTag("UiElement"))
					{
						if (Time.time - tempsUI > 0.1f) {
							device.TriggerHapticPulse (1000);

							tempsUI = Time.time;
							currentFocus.GetComponent<UiElement>().goal.ActivateMenu();
							currentFocus.GetComponent<UiElement>().PerformAction(this);
						}
					}					
					else if (currentFocus.CompareTag("InfoPanel"))
					{
						device.TriggerHapticPulse (1000);
						currentFocus.GetComponent<Infopanel> ().CloseInfoPanel ();
					}
					else if (currentFocus.CompareTag("TeleportTrigger"))
					{
						Teleportation.Instance.JumpToPos(currentFocus.GetComponent<TeleportationTrigger>().triggerPos);
					}

				} else
				{
					// user is clicking somewhere outside to close the menu
					// test how it is if we not use that

					/*

					// check that we are not in group selection
					if (!groupItemSelection) {
						UiCanvasGroup.Instance.CloseMenu(this);
					
						DeAssignCurrentSelection(currentSelection);
						this.enableFaceSelection(false);
						otherController.enableFaceSelection(false);
					}
					*/
				}

				temps = 0;
			}
		}
        
    }

	public void ActivateController(bool value){
		
		LaserPointer.transform.GetChild(0).gameObject.SetActive(value);
		controllerActive = value;
	}

    public void SelectLatestObject()
    {
		// deselect current object

        // directly select new object
        UiCanvasGroup.Instance.Show();
        ObjectsManager.Instance.GetlatestObject().Select(this, uiPositon); 
    }

    public void FindCollidingFace(Vector3 pointOfCollision){
		collidingFace = UiCanvasGroup.Instance.currentModelingObject.GetFaceFromCollisionCoordinate (pointOfCollision);
	}

    public void CreatePointOfCollisionPrefab()
    {
        if (pointOfCollisionGO == null)
        {
            pointOfCollisionGO = (GameObject)Instantiate(pointOfCollisionPrefab, pointOfCollision, new Quaternion(0f, 0f, 0, 0f));
            pointOfCollisionGO.transform.SetParent(transform);
        }
    }

    public void AssignCurrentFocus(GameObject newCollider)
    {
		currentFocus = newCollider;
        temps = 0f;
    }

    public void DeAssignCurrentFocus(GameObject collider)
    {
        if (currentFocus == collider)
        {
            currentFocus = null;
        }

        temps = 0f;
    }

    public void AssignCurrentSelection(GameObject newCollider)
    {
        currentSelection = newCollider;
    }

    public void DeAssignCurrentSelection(GameObject collider)
    {
        if (currentSelection == collider)
        {
            currentSelection = null;
        }
    }

	public void enableFaceSelection(bool value){

        if (value == false && collidingFace!=null)
        {
            collidingFace.UnHighlight();
        }

		faceSelection = value;	
	}

    public bool GetObjectMoving()
    {
        return movingObject;
    }

}
