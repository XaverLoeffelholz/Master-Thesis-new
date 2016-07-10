using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Group : MonoBehaviour {

    public List<ModelingObject> objectList = new List<ModelingObject>();
    public Vector3 ScalingCenter;
    public Vector3 RotationCenter;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 GetBoundingBoxTopCenter()
    {
        // add all bottom centers of objects together
        Vector3 center = Vector3.zero;
        float y = -9999f;

        for (int i = 0; i < objectList.Count; i++)
        {
            center += objectList[i].GetBoundingBoxTopCenter();

            if (objectList[i].GetBoundingBoxBottomCenter().y > y)
            {
                y = objectList[i].GetBoundingBoxBottomCenter().y;
            }
        }

        center = center / objectList.Count;

        return new Vector3(center.x, y, center.z);
    }

    public Vector3 GetBoundingBoxBottomCenter()
    {
        Vector3 center = Vector3.zero;
        float y = 9999f;

        for (int i = 0; i < objectList.Count; i++)
        {
            center += objectList[i].GetBoundingBoxBottomCenter();

            if (objectList[i].GetBoundingBoxBottomCenter().y < y)
            {
                y = objectList[i].GetBoundingBoxBottomCenter().y;
            }
        }

        // calculate center
        center = center / objectList.Count;

        return new Vector3(center.x, y, center.z);
    }

    public Vector3 GetBoundingBoxCenter()
    {
        Vector3 center = Vector3.zero;

        // add all centers together and divide by number of objects
        for (int i = 0; i < objectList.Count; i++)
        {
            center += objectList[i].GetBoundingBoxCenter();
        }

        center = center / objectList.Count;

        return center;
    }


    public void FocusGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++) {         
            if (objectList[i] != initiater)
            {
                objectList[i].Highlight();
            }
        }
    }

    public void UnFocusGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].UnHighlight();
            }
        }
    }

    public void RotateGroup(ModelingObject initiater, Vector3 angleAxis, float angle)
    {
        // use
        RotationCenter = GetBoundingBoxCenter();

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].RotateAround(angleAxis, angle);
            }
        }

    }

    public void DuplicateGroup()
    {
        // create new group
        Group newGroup;
        newGroup = ObjectsManager.Instance.CreateGroup();

        // Go through all objects, duplicate each
        for (int i = 0; i < objectList.Count; i++)
        {
            ObjectCreator.Instance.DuplicateObject(objectList[i], newGroup);
        }

        // problem: Objects all just on top of other elements
    }

    public void SelectGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].ShowOutline(true);
            }
        }

    }

    public void DeSelectGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].ShowOutline(false);
            }
        }

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

    public void StopMoving(Selection controller, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].StopMoving(controller, initiater);
            }
        }
    }

    public void StartScalingGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].StartScaling(false);
            }
        }
    }

    public void ScaleBy(float newScale, ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].ScaleBy(newScale, false);
            }
        }
    }

    public void TrashGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
                objectList[i].TrashObject(false);
            }
        }
    }

  
}
