using UnityEngine;
using System.Collections;

public class library : Singleton<library>{

	private Vector3 initialPosition;

	private Selection controllerForMovement;
	private Vector3 lastPositionController;
	private bool moving = false;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

    // Use this for initialization
    void Start () {
		initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Focus(Selection controller){

	}
	public void UnFocus(Selection controller){

	}

    public void ClearLibrary()
    {
        foreach(Transform modelingObject in transform)
        {
            if(modelingObject.CompareTag("ModelingObject")){
                Destroy(modelingObject.gameObject);
            }
        }

        Invoke("RefillLibrary", 1.0f);
    }

    public void RefillLibrary()
    {
        ObjectCreator.Instance.createSetofObjects();
    }

	// library should be movable as objects

	// what if object intersects with object


	public void StartMoving(Selection controller)
	{
		moving = true;
		controllerForMovement = controller;
		lastPositionController = controller.pointOfCollisionGO.transform.position;
	}

	public void StopMoving(Selection controller)
	{
		moving = false;
		controllerForMovement = null;
	}


	// Update is called once per frame
	void FixedUpdate () {
		if (moving) {
			Vector3 prevPosition = transform.position;
			Vector3 newPositionController = controllerForMovement.pointOfCollisionGO.transform.position;

			Vector3 newPositionWorld = prevPosition + (newPositionController - lastPositionController);
			lastPositionController = newPositionController;
			this.transform.position = new Vector3(newPositionWorld.x, newPositionWorld.y, newPositionWorld.z);
		}
	}


}
