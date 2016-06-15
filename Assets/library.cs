using UnityEngine;
using System.Collections;

public class library : Singleton<library>{

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
            Destroy(modelingObject.gameObject);
        }
    }
}
