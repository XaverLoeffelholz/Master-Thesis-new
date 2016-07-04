using UnityEngine;
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
        createNewObject(square, ModelingObject.ObjectType.square, null, new Vector3(0, 0.5f, 0f), true);
        ObjectCreator.Instance.createSetofObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void createSetofObjects()
    {
        createNewObject(triangle, ModelingObject.ObjectType.triangle, null, new Vector3(2f, 2.7f, 7.6f), false);
        createNewObject(square, ModelingObject.ObjectType.square, null, new Vector3(3.3f, 2.5f, 6.7f), false);
        createNewObject(hexagon, ModelingObject.ObjectType.hexagon, null, new Vector3(4.4f, 2.1f, 5.3f), false);
        createNewObject(octagon, ModelingObject.ObjectType.octagon, null, new Vector3(4.9f, 1.7f, 3.4f), false);
    }

	public void createNewObject(Mesh mesh, ModelingObject.ObjectType type, Face groundface, Vector3 offSet, bool insideStage)
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
            newModelingObject.topFace.ReplaceFaceOnOtherFace (groundface, groundface.normal*0.4f, false);

            // recalculate centers
            newModelingObject.RecalculateCenters();

            // recalculate normals
            newModelingObject.RecalculateNormals();
        }
        else {

            newObject.transform.localScale = new Vector3(1f, 1f, 1f);

            if (insideStage)
            {
                newObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                newObject.transform.localPosition = newObject.transform.localPosition + offSet;
            } else
            {
                newObject.transform.position = library.transform.position + (offSet * library.localScale.x);
            }

        }

       // newModelingObject.CorrectOffset();
       newModelingObject.InitiateHandles();

    }


	public void createNewObjectOnFace(Face groundface) {

		// get number of vertices
		int numberOfVertices = groundface.vertexBundles.Length;
		Vector3 offset = new Vector3(0f,0f,0f);

		switch (numberOfVertices) 
		{
		case 3:
			createNewObject (triangle, ModelingObject.ObjectType.triangle, groundface, offset, true);
            break;
		case 4:
			createNewObject (square, ModelingObject.ObjectType.square, groundface, offset, true);
            break;
		case 6:
			createNewObject (hexagon, ModelingObject.ObjectType.hexagon, groundface, offset, true);
            break;
		case 8:
			createNewObject (octagon, ModelingObject.ObjectType.octagon, groundface, offset, true);
            break;
		}
	}

    public void DuplicateObject(ModelingObject original)
    {
        Vector3 position = original.topFace.center.transform.GetChild(0).position;
        position = position + (position - original.bottomFace.center.transform.GetChild(0).position);

        createNewObject(original.mesh, original.typeOfObject, null, position, true);
    }


}
