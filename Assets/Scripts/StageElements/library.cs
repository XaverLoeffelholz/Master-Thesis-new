using UnityEngine;
using System.Collections;

public class library : Singleton<library>{

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClearLibrary()
    {
        foreach(Transform modelingObject in transform)
        {
            if(modelingObject.CompareTag("ModelingObject")){
                Destroy(modelingObject.gameObject);
            }
        }

        Invoke("RefillLibrary", 1.3f);
    }

    public void RefillLibrary()
    {
        ObjectCreator.Instance.createSetofObjects();
    }
}
