using UnityEngine;
using System.Collections;

public class ObjectsManager : Singleton<ObjectsManager>
{
    // restructure, create List of Objects

    public Transform DistanceVisualisation;
    public GameObject groupPrefab;
    public Group currentGroup;
    public GameObject user;
	public Transform stage;
	public Transform stageScaler;

    // Use this for initialization
    void Start () {
        user = ObjectsManager.Instance.user;
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

    public Group CreateGroup()
    {
        GameObject newgroup = Instantiate(groupPrefab);
        newgroup.transform.SetParent(transform);

        newgroup.transform.localPosition = Vector3.zero;
        newgroup.transform.localEulerAngles = Vector3.zero;
        newgroup.transform.localScale = Vector3.one;

        currentGroup = newgroup.GetComponent<Group>();
        return currentGroup;
    }

    public void AddObjectToGroup(Group group, ModelingObject modelingObject)
    {
        modelingObject.transform.SetParent(group.transform);
        modelingObject.group = group;
        group.objectList.Add(modelingObject);
		group.DrawBoundingBox ();
    }

    public void TakeObjectOutOfGroup(Group group, ModelingObject modelingObject)
    {
        modelingObject.transform.SetParent(group.transform.parent);
        modelingObject.group = null;
        group.objectList.Remove(modelingObject);
		group.DrawBoundingBox ();
    }

    public void ReOpenGroup(Group group)
    {
        currentGroup = group.GetComponent<Group>();
    }
}
