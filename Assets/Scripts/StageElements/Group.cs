﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Group : MonoBehaviour {

    public List<ModelingObject> objectList = new List<ModelingObject>();
    public Vector3 ScalingCenter;
    public Vector3 RotationCenter;

	public BoundingBox boundingBox;
	public bool focused;

	public ObjectSelecter objectSelector;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InitiateGroup(){

	}

	public void BreakGroup (){
		for (int i = 0; i < objectList.Count; i++) {         
			objectList [i].transform.SetParent (ObjectsManager.Instance.transform);
			objectList [i].group = null;
		}

		ObjectsManager.Instance.DeleteGroup (this);
	}

	public void UpdateBoundingBox(){
		boundingBox.coordinates = new Vector3[8];

		// get highest and lowest values for x,y,z
		Vector3 minima = GetBoundingBoxMinima();
		Vector3 maxima = GetBoundingBoxMaxima();

		// set all points
		boundingBox.coordinates[0] = transform.TransformPoint(new Vector3(maxima.x,maxima.y,maxima.z));
		boundingBox.coordinates[1] = transform.TransformPoint(new Vector3(maxima.x,maxima.y,minima.z));
		boundingBox.coordinates[2] = transform.TransformPoint(new Vector3(minima.x,maxima.y,minima.z));
		boundingBox.coordinates[3] = transform.TransformPoint(new Vector3(minima.x,maxima.y,maxima.z));

		boundingBox.coordinates[4] = transform.TransformPoint(new Vector3(maxima.x,minima.y,maxima.z));
		boundingBox.coordinates[5] = transform.TransformPoint(new Vector3(maxima.x,minima.y,minima.z));
		boundingBox.coordinates[6] = transform.TransformPoint(new Vector3(minima.x,minima.y,minima.z));
		boundingBox.coordinates[7] = transform.TransformPoint(new Vector3(minima.x,minima.y,maxima.z));
	}

	public void DrawBoundingBox(){
		UpdateBoundingBox ();

		for (int i = 0; i < objectList.Count; i++)
		{
			objectList [i].boundingBox.ClearBoundingBox ();
		}

		boundingBox.DrawBoundingBox ();
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
			center += objectList[i].transform.TransformPoint(objectList[i].GetBoundingBoxBottomCenter());

			if (objectList[i].transform.TransformPoint(objectList[i].GetBoundingBoxBottomCenter()).y < y)
            {
				y = objectList[i].transform.TransformPoint(objectList[i].GetBoundingBoxBottomCenter()).y;
            }
        }

        // calculate center
        center = center / objectList.Count;

	//	Debug.Log ("lowest point:" + new Vector3 (center.x, y, center.z));

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

	public Vector3 GetBoundingBoxMinima()
	{
		Vector3 minima = new Vector3 (9999f, 9999f, 9999f);

		for (int i = 0; i < objectList.Count; i++)
		{
			Vector3 currentMinima = transform.InverseTransformPoint(objectList [i].transform.TransformPoint(objectList [i].GetBoundingBoxMinima ()));

			if (currentMinima.x < minima.x) {
				minima.x = currentMinima.x;
			}

			if (currentMinima.y < minima.y) {
				minima.y = currentMinima.y;
			}

			if (currentMinima.z < minima.z) {
				minima.z = currentMinima.z;
			}

		}

		return minima;
	}

	public Vector3 GetBoundingBoxMaxima()
	{
		Vector3 maxima = new Vector3 (-9999f, -9999f, -9999f);

		for (int i = 0; i < objectList.Count; i++)
		{
			Vector3 currentMaxima = transform.InverseTransformPoint(objectList [i].transform.TransformPoint(objectList [i].GetBoundingBoxMaxima ()));

			if (currentMaxima.x > maxima.x) {
				maxima.x = currentMaxima.x;
			}

			if (currentMaxima.y > maxima.y) {
				maxima.y = currentMaxima.y;
			}

			if (currentMaxima.z > maxima.z) {
				maxima.z = currentMaxima.z;
			}

		}

		return maxima;
	}


	public void FocusGroup(ModelingObject initiater, Selection controller)
    {
		focused = true;

		objectSelector.ShowSelectionButton (controller);

        for (int i = 0; i < objectList.Count; i++) {         
            if (objectList[i] != initiater)
            {
				objectList[i].Focus(controller);
            }
        }
    }

	public void UnFocusGroup(ModelingObject initiater, Selection controller)
    {
		focused = false;

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
				objectList[i].UnFocus(controller);
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
          //  ObjectCreator.Instance.DuplicateObject(objectList[i], newGroup);
        }

        // problem: Objects all just on top of other elements
    }

    public void SelectGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
               // objectList[i].ShowOutline(true);
            }
        }

		DrawBoundingBox ();

    }

    public void DeSelectGroup(ModelingObject initiater)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != initiater)
            {
               // objectList[i].ShowOutline(false);
            }
        }

		boundingBox.ClearBoundingBox ();

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

	public void ColorGroup(ModelingObject initiater, Color color){
		for (int i = 0; i < objectList.Count; i++)
		{
			if (objectList[i] != initiater)
			{
				objectList[i].ChangeColor(color, false);
			}
		}
	}
  
	public Vector3 GetPosOfClosestVertex(Vector3 position, Vector3[] coordinates){
		
		Vector3 closestVertex = coordinates[0];
		float shortestDistance = 999999f;

		for (int i = 0; i < coordinates.Length; i++)
		{
			Vector3 newCoordinate = coordinates[i];

			if (Vector3.Distance(position, newCoordinate) < shortestDistance)
			{
				closestVertex = newCoordinate;
				shortestDistance = Vector3.Distance(position, newCoordinate);
			}
		}

		return closestVertex;
	}
		
	
}
