﻿using UnityEngine;
using System.Collections;

public class ObjectCreator : Singleton<ObjectCreator> {
	
    public GameObject modelingObject;
    public Mesh triangle;
    public Mesh square;
    public Mesh hexagon;
    public Mesh octagon;

    public Transform objects;
    public Transform library;

    private int objectIDcount = 0;


    // Use this for initialization
    void Start () {
        createNewObject(square, ModelingObject.ObjectType.square, null, null, new Vector3(0, 0.5f, 0f), true);
        ObjectCreator.Instance.createSetofObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void createSetofObjects()
    {
        /*
        createNewObject(triangle, ModelingObject.ObjectType.triangle, null, null, new Vector3(2.4f, 5f, 7.8f), false);
        createNewObject(square, ModelingObject.ObjectType.square, null, null, new Vector3(3.7f, 4.8f, 7.0f), false);
        createNewObject(hexagon, ModelingObject.ObjectType.hexagon, null, null, new Vector3(4.8f, 4.4f, 5.5f), false);
        createNewObject(octagon, ModelingObject.ObjectType.octagon, null, null, new Vector3(5.3f, 4.1f, 3.6f), false);
        */

        createNewObject(triangle, ModelingObject.ObjectType.triangle, null, null, library.GetComponent<library>().pos1.localPosition, false);
        createNewObject(square, ModelingObject.ObjectType.square, null, null, library.GetComponent<library>().pos2.localPosition, false);
        createNewObject(hexagon, ModelingObject.ObjectType.hexagon, null, null, library.GetComponent<library>().pos3.localPosition, false);
        createNewObject(octagon, ModelingObject.ObjectType.octagon, null, null, library.GetComponent<library>().pos4.localPosition, false);
    }

	public void createNewObject(Mesh mesh, ModelingObject.ObjectType type, Face groundface, ModelingObject original, Vector3 offSet, bool insideStage)
    {
        GameObject newObject = new GameObject();
        newObject = Instantiate(modelingObject);
        if (insideStage)
        {
            newObject.transform.SetParent(objects);
        } else
        {
            newObject.transform.SetParent(library);
        }

        ModelingObject newModelingObject = newObject.GetComponent<ModelingObject>();

        newObject.name = "Object " + objectIDcount;
        newModelingObject.ObjectID = objectIDcount;
        objectIDcount++;

        newModelingObject.typeOfObject = type;

        newObject.GetComponent<ModelingObject>().Initiate(mesh);

        if (groundface != null) {

            newObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            newObject.transform.position = groundface.center.transform.TransformPoint(groundface.center.coordinates + groundface.normal*0.4f);
            newObject.transform.localScale = groundface.transform.parent.parent.transform.localScale;

            newModelingObject.bottomFace.ReplaceFaceOnOtherFace (groundface, new Vector3 (0f, 0f, 0f), true);
            newModelingObject.topFace.ReplaceFaceOnOtherFace (groundface, groundface.normal.normalized*0.1f, false);

            // recalculate centers
            newModelingObject.RecalculateCenters();

            // recalculate normals
            newModelingObject.RecalculateNormals();
        }
        else {
            if (insideStage)
            {
                newObject.transform.localScale = new Vector3(1f, 1f, 1f);
                newObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                newObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                newObject.transform.localPosition = newObject.transform.localPosition + offSet;
            } else
            {
                newObject.transform.position = library.transform.position + (offSet * library.localScale.x);
                newObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }

        }

       // newModelingObject.CorrectOffset();
       newModelingObject.InitiateHandles();

        if (original != null)
        {
            newModelingObject.SetVertexBundlePositions(original);
        }
    }


	public void createNewObjectOnFace(Face groundface) {

		// get number of vertices
		int numberOfVertices = groundface.vertexBundles.Length;
		Vector3 offset = new Vector3(0f,0f,0f);

		switch (numberOfVertices) 
		{
		case 3:
			createNewObject (triangle, ModelingObject.ObjectType.triangle, groundface, null, offset, true);
            break;
		case 4:
			createNewObject (square, ModelingObject.ObjectType.square, groundface, null, offset, true);
            break;
		case 6:
			createNewObject (hexagon, ModelingObject.ObjectType.hexagon, groundface, null, offset, true);
            break;
		case 8:
			createNewObject (octagon, ModelingObject.ObjectType.octagon, groundface, null, offset, true);
            break;
		}
	}

    public void DuplicateObject(ModelingObject original)
    {
        Vector3 localPosition = original.topFace.centerPosition;
        localPosition = localPosition + (original.transform.localPosition - original.bottomFace.centerPosition);

        if (original.typeOfObject == ModelingObject.ObjectType.triangle)
        {
            createNewObject(triangle, original.typeOfObject, null, original, localPosition, true);
        } else if (original.typeOfObject == ModelingObject.ObjectType.square)
        {
            createNewObject(square, original.typeOfObject, null, original, localPosition, true);
        }
        else if (original.typeOfObject == ModelingObject.ObjectType.hexagon)
        {
            createNewObject(hexagon, original.typeOfObject, null, original, localPosition, true);
        }
        else if (original.typeOfObject == ModelingObject.ObjectType.octagon)
        {
            createNewObject(octagon, original.typeOfObject, null, original, localPosition, true);
        }


    }


}