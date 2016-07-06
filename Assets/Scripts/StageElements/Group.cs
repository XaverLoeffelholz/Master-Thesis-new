using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Group : MonoBehaviour {

    public List<ModelingObject> objectList = new List<ModelingObject>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FocusGroup()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
          //  objectList[i].Focus(controller);
        }
    }

    public void UnFocusGroup()
    {

    }

    public void RotateGroup()
    {

    }

    public void DuplicateGroup()
    {

    }

    public void SelectGroup()
    {
        
    }

    public void DeSelectGroup()
    {

    }

    public void Move(Vector3 distance, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                // apply a similar movement
                objectList[i].MoveModelingObject(distance);
            }
        }
    }

    public void StartMoving(Selection controller, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                // apply a similar movement
            }
        }
    }

    public void StopMoving(Selection controller, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
          //  objectList[i].StopMoving(controller, initiater);
        }
    }

    public void StartScalingGroup()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].StartScaling();
        }
    }

    public void TrashGroup()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].TrashObject();
        }
    }

  
}
