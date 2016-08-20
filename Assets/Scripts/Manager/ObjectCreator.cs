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

	public Color standardColor;

    private int objectIDcount = 0;

	public ModelingObject latestModelingObject;


    // Use this for initialization
    void Start () {
		//createNewObject(triangle, ModelingObject.ObjectType.triangle, null, null, new Vector3(-2, 0.3f, 0f), true, null, standardColor);
		createNewObject(square, ModelingObject.ObjectType.square, null, null, new Vector3(0f, 0f, 0f), true, null, standardColor);
		//createNewObject(octagon, ModelingObject.ObjectType.octagon, null, null, new Vector3(2, 0.3f, 0f), true, null, standardColor);
        ObjectCreator.Instance.createSetofObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void createSetofObjects()
    {
		createNewObject(triangle, ModelingObject.ObjectType.triangle, null, null, library.GetComponent<library>().pos1.position, false, null, standardColor);       
		createNewObject(square, ModelingObject.ObjectType.square, null, null, library.GetComponent<library>().pos2.position, false, null, standardColor);
		createNewObject(octagon, ModelingObject.ObjectType.octagon, null, null, library.GetComponent<library>().pos3.position, false, null, standardColor);

        //createNewObject(hexagon, ModelingObject.ObjectType.hexagon, null, null, library.GetComponent<library>().pos3.localPosition, false, null);		
    }

	public void createNewObject(Mesh mesh, ModelingObject.ObjectType type, Face groundface, ModelingObject original, Vector3 offSet, bool insideStage, Group group, Color color)
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
				newObject.transform.localPosition = newObject.transform.localPosition + new Vector3(0, 0.3f, 0f);
            } else
            {
				newObject.transform.position = offSet;
                newObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

        }

        if (original != null)
        {
            newModelingObject.SetVertexBundlePositions(original);
			newObject.transform.position = offSet;
        }

        // newModelingObject.CorrectOffset();
        newModelingObject.CalculateBoundingBox();
        newModelingObject.InitiateHandles();

        if (group != null) {
            ObjectsManager.Instance.AddObjectToGroup(group, newModelingObject);
			newObject.transform.position = offSet;
        }

		newModelingObject.ChangeColor(color, true);

		latestModelingObject = newModelingObject;
    }


	public void createNewObjectOnFace(Face groundface) {

		// get number of vertices
		int numberOfVertices = groundface.vertexBundles.Length;
		Vector3 offset = new Vector3(0f,0f,0f);

		switch (numberOfVertices) 
		{
		case 3:
			createNewObject (triangle, ModelingObject.ObjectType.triangle, groundface, null, offset, true, null, standardColor);
                break;
		case 4:
			createNewObject (square, ModelingObject.ObjectType.square, groundface, null, offset, true, null, standardColor);
                break;
		case 6:
			createNewObject (hexagon, ModelingObject.ObjectType.hexagon, groundface, null, offset, true, null, standardColor);
                break;
		case 8:
			createNewObject (octagon, ModelingObject.ObjectType.octagon, groundface, null, offset, true, null, standardColor);
                break;
		}
	}

	public void DuplicateObject(ModelingObject original, Group group, Vector3 objectPosition)
    {
        if (original.typeOfObject == ModelingObject.ObjectType.triangle)
        {
			createNewObject(triangle, original.typeOfObject, null, original, objectPosition, true, group, original.currentColor);
        } else if (original.typeOfObject == ModelingObject.ObjectType.square)
        {
			createNewObject(square, original.typeOfObject, null, original, objectPosition, true, group, original.currentColor);
        }
        else if (original.typeOfObject == ModelingObject.ObjectType.hexagon)
        {
			createNewObject(hexagon, original.typeOfObject, null, original, objectPosition, true, group, original.currentColor);
        }
        else if (original.typeOfObject == ModelingObject.ObjectType.octagon)
        {
			createNewObject(octagon, original.typeOfObject, null, original, objectPosition, true, group, original.currentColor);
        }
    }


}
