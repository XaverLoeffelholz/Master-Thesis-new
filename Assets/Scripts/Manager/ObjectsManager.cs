using UnityEngine;
using System.Collections;

public class ObjectsManager : Singleton<ObjectsManager>
{
    // restructure, create List of Objects

    public Transform DistanceVisualisation;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HideAllHandles()
    {
        foreach(Transform model in this.transform)
        {
            if (model.CompareTag("ModelingObject"))
            {
                model.GetComponent<ModelingObject>().handles.DisableHandles();
            }

        }
    }

    public ModelingObject GetlatestObject()
    {
        int i = 0;

        foreach (Transform model in this.transform)
        {
            if (model.CompareTag("ModelingObject"))
            {
              if (model.GetComponent<ModelingObject>().ObjectID > i)
                {
                    i = model.GetComponent<ModelingObject>().ObjectID;
                }
            }
        }

        ModelingObject m = GameObject.Find("Object " + i).GetComponent<ModelingObject>();

        return m;
    }
}
