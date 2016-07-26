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
	private bool faceSelection;

    [HideInInspector]
    public GameObject pointOfCollisionGO;

    SteamVR_TrackedObject trackedObj;

    private Vector3 uiPositon;
    public bool groupItemSelection = false;

    public GameObject cam;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        if (typeOfController == controllerType.SecondaryController)
        {
            LaserPointer.transform.GetChild(0).gameObject.SetActive(false);
        }
    }


	void DeFocusCurrent(GameObject newObject){
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
			} else if (currentFocus.CompareTag ("SelectionButton")) {
				
				// compare selection button and object
				if (newObject.CompareTag ("ModelingObject")) {
					if (currentFocus.GetComponent<ObjectSelecter> ().connectedObject != newObject.GetComponent<ModelingObject> ()) {
						currentFocus.GetComponent<ObjectSelecter> ().UnFocus (this);
						currentFocus.GetComponent<ObjectSelecter> ().connectedObject.UnFocus (this);
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

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            ObjectCreator.Instance.createSetofObjects();
        }
        
        RaycastHit hit;

        // only change focus is the object is not moved at the moment
		if (!movingObject && !movingHandle && !scalingObject && !triggerPressed) {
			if (Physics.Raycast (LaserPointer.transform.position, LaserPointer.transform.forward, out hit)) {
				if (currentFocus != null) {
					Vector3 directionLaserPointerObject = (currentFocus.transform.position - LaserPointer.transform.position).normalized;
					uiPositon = LaserPointer.transform.position + directionLaserPointerObject * 0.6f + Vector3.down * 0.25f;
				}

				AdjustLengthPointer (hit.distance);

				if (hit.rigidbody != null && hit.rigidbody.transform.parent != null) {
					if (currentFocus != hit.rigidbody.transform.parent.gameObject) {
						// focus of Object
						if (hit.rigidbody.transform.parent.gameObject.CompareTag ("ModelingObject")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<ModelingObject> ().Focus (this);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("Handle")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<handle> ().Focus (this);
							device.TriggerHapticPulse (300);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("UiElement")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<UiElement> ().Focus (this);
							device.TriggerHapticPulse (600);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("TeleportTrigger")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<TeleportationTrigger> ().Focus (this);
							device.TriggerHapticPulse (300);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("SelectionButton")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<ObjectSelecter> ().connectedObject.Focus (this);
							device.TriggerHapticPulse (600);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("TeleportPosition")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							currentFocus.GetComponent<TeleportationPosition> ().Focus (this);
							device.TriggerHapticPulse (300);
						} else if (hit.rigidbody.transform.parent.gameObject.CompareTag ("Library")) {
							DeFocusCurrent (hit.rigidbody.transform.parent.gameObject);
							currentFocus = hit.rigidbody.transform.parent.gameObject;
							library.Instance.Focus (this);
							device.TriggerHapticPulse (300);
						}
					}

					// Set position of collision
					pointOfCollision = hit.point;

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

			if (triggerPressed && temps > 0.1f && (currentFocus.CompareTag("ModelingObject") || currentFocus.CompareTag("TeleportPosition") || currentFocus.CompareTag("Library") ))
            {
				if (!movingObject && !faceSelection && !groupItemSelection){
					
					CreatePointOfCollisionPrefab();
					movingObject = true;
					this.GetComponent<StageController> ().ShowPullVisual (true);

                    // here we should also change the icon on the other controller to a toggle where we can choose between rotation and scaling

                    //!!!!

					UiCanvasGroup.Instance.Hide();

					if (currentFocus.CompareTag ("ModelingObject")) {	
						currentFocus.GetComponent<ModelingObject>().StartMoving(this, currentFocus.GetComponent<ModelingObject>());
                        otherController.LaserPointer.transform.GetChild(0).gameObject.SetActive(true);
						//currentFocus.GetComponent<ModelingObject> ().DeSelect (this);

					} else if (currentFocus.CompareTag ("TeleportPosition")) {
						currentFocus.GetComponent<TeleportationPosition>().StartMoving(this);
					} else if (currentFocus.CompareTag ("Library")) {
						library.Instance.StartMoving(this);
					}
				}


			} 
            else if (triggerPressed && (movingHandle || currentFocus.CompareTag("Handle")))
            {
                if (pointOfCollisionGO == null)
                {
                    CreatePointOfCollisionPrefab();
                }
                currentFocus.GetComponent<handle>().ApplyChanges(pointOfCollisionGO);
                movingHandle = true;
            }
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            triggerPressed = false;
            movingHandle = false;

            if (movingObject)
			{
				if (currentFocus.CompareTag ("ModelingObject")) {	
					currentFocus.GetComponent<ModelingObject>().StopMoving(this, currentFocus.GetComponent<ModelingObject>());

					if (currentFocus.GetComponent<ModelingObject>().inTrashArea)
					{
						currentFocus.GetComponent<ModelingObject>().TrashObject(true);
						device.TriggerHapticPulse(1000);
					}
				}

				if (currentFocus.CompareTag ("TeleportPosition")) {
					currentFocus.GetComponent<TeleportationPosition>().StopMoving(this);
					Teleportation.Instance.JumpToPos(5);
				}

				if (currentFocus.CompareTag ("Library")) {
					library.Instance.StopMoving(this);
				}

            }

            Destroy(pointOfCollisionGO);
            movingObject = false;
			this.GetComponent<StageController> ().ShowPullVisual (false);

            if (currentFocus != null)
            {
                if (currentFocus.CompareTag("ModelingObject"))
                {
                    if (!groupItemSelection && currentSelection != null && currentSelection != currentFocus)
                    {
                        currentSelection.GetComponent<ModelingObject>().DeSelect(this);
                        UiCanvasGroup.Instance.Hide();
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
                    currentFocus.GetComponent<handle>().ResetLastPosition();
                    currentFocus.GetComponent<handle>().UnLock();
                    currentFocus.GetComponent<handle>().UnFocus(this);
                } else if (currentFocus.CompareTag("UiElement"))
                {
                    currentFocus.GetComponent<UiElement>().goal.ActivateMenu();
                    currentFocus.GetComponent<UiElement>().PerformAction(this);
                }
                else if (currentFocus.CompareTag("TeleportTrigger"))
                {
                    Teleportation.Instance.JumpToPos(currentFocus.GetComponent<TeleportationTrigger>().triggerPos);
                }

            } else
            {
				// user is clicking somewhere outside to close the menu

				// check that we are not in group selection
				if (!groupItemSelection) {
					UiCanvasGroup.Instance.Hide();

					if (currentSelection != null)
					{
						currentSelection.GetComponent<ModelingObject>().DeSelect(this);
					}

					DeAssignCurrentSelection(currentSelection);
					this.enableFaceSelection(false);
					otherController.enableFaceSelection(false);
					ObjectsManager.Instance.HideAllHandles();
				}

            }

            temps = 0;
        }
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
            pointOfCollisionGO.transform.SetParent(LaserPointer.transform);
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
