using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Selection : MonoBehaviour
{
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

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }


    void DeFocusCurrent(){

		if (currentFocus != null) {
			if (currentFocus.CompareTag ("ModelingObject")) {
				currentFocus.GetComponent<ModelingObject> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("Handle")) {
				currentFocus.GetComponent<handle> ().UnFocus (this);
			} else if (currentFocus.CompareTag ("UiElement")) {
				currentFocus.GetComponent<UiElement> ().UnFocus (this);
			}

			currentFocus = null;
		}

	}

    public void AdjustLengthPointer(float MaxDistance)
    {
        LaserPointer.transform.localScale = new Vector3 (1,1,Mathf.Clamp(MaxDistance, 0f,50f));
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            ObjectCreator.Instance.createSetofObjects();
        }
        
        RaycastHit hit;

        // only change focus is the object is not moved at the moment
        if (!movingObject && !movingHandle && !scalingObject)
        {
            if (Physics.Raycast(LaserPointer.transform.position, LaserPointer.transform.forward, out hit))
            {

                AdjustLengthPointer(hit.distance);

                if (hit.rigidbody != null)
                {
                    if (currentFocus != hit.rigidbody.transform.parent.gameObject)
                    {
                        DeFocusCurrent();
                        // Set new focus
                        currentFocus = hit.rigidbody.transform.parent.gameObject;

                        // focus of Object
                        if (currentFocus.CompareTag("ModelingObject"))
                        {
                            currentFocus.GetComponent<ModelingObject>().Focus(this);
                        }
                        else if (currentFocus.CompareTag("Handle"))
                        {
                            currentFocus.GetComponent<handle>().Focus(this);
                        }
                        else if (currentFocus.CompareTag("UiElement"))
                        {
                            currentFocus.GetComponent<UiElement>().Focus(this);
                        }
                    }

                    // Set position of collision
                    pointOfCollision = hit.point;

                    if (faceSelection)
                    {
                        // highlight face that is selected
                        if (collidingFace != null)
                        {
                            collidingFace.UnHighlight();
                        }
                        
                        FindCollidingFace(pointOfCollision);

                        if (collidingFace != null)
                        {
                            collidingFace.Highlight();
                        }

                    }

                }
                else
                {

                    if (currentFocus != null)
                    {
                        DeFocusCurrent();
                    }
                }
            }
            else
            {
                if (currentFocus != null)
                {
                    DeFocusCurrent();
                }

                AdjustLengthPointer(50f);
            }
        }
		
			

        if (currentFocus != null)
        {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = true;
                temps = Time.time;
            }

            if (triggerPressed && currentFocus.CompareTag("ModelingObject") && Time.time - temps > 0.4f && !movingObject && !faceSelection)
            {
                CreatePointOfCollisionPrefab();
                movingObject = true;
                currentFocus.GetComponent<ModelingObject>().StartMoving(this);
                UiCanvasGroup.Instance.Hide();
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
                currentFocus.GetComponent<ModelingObject>().StopMoving(this);
            }

            Destroy(pointOfCollisionGO);
            movingObject = false;

            if (currentFocus != null)
            {
                if (currentFocus.CompareTag("ModelingObject") && Time.time - temps <= 0.4f)
                {
                    if (currentSelection != null)
                    {
                        currentSelection.GetComponent<ModelingObject>().DeSelect(this);
                    }

                    if (!faceSelection) {
						UiCanvasGroup.Instance.Show ();
                        currentFocus.GetComponent<ModelingObject> ().Select (this);
						collidingFace = null;
					} else {
                        this.enableFaceSelection(false);
                        otherController.enableFaceSelection(false);
                        if (collidingFace != null)
                        {
                            collidingFace.CreateNewModelingObject();
                        }


                        // directly select new object
                        UiCanvasGroup.Instance.Show();
                        ObjectsManager.Instance.GetlatestObject().Select (this);
                        collidingFace = null;

                    }


                } else if (currentFocus.CompareTag("Handle"))
                {
                    currentFocus.GetComponent<handle>().ResetLastPosition();
                    currentFocus.GetComponent<handle>().UnFocus(this); 

                } else if (currentFocus.CompareTag("UiElement"))
                {
                    currentFocus.GetComponent<UiElement>().goal.ActivateMenu();
				} 

            } else
            {
                UiCanvasGroup.Instance.Hide();
                DeAssignCurrentSelection(currentSelection);
                this.enableFaceSelection(false);
                otherController.enableFaceSelection(false);
                ObjectsManager.Instance.HideAllHandles();

                if (currentSelection != null)
                {
                    currentSelection.GetComponent<ModelingObject>().DeSelect(this);
                }
            }

            temps = 0;
        }
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
