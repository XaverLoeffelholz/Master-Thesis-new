using UnityEngine;
using System.Collections;

public class Trash : MonoBehaviour {
    private Color initialColor;

	// Use this for initialization
	void Start () {
        initialColor = transform.GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        transform.GetComponent<Renderer>().material.color = new Color(0.7f, 0.1f, 0.1f, 0.6f);

        if (other.gameObject.CompareTag("Mesh"))
        {
            other.transform.parent.GetComponent<ModelingObject>().inTrashArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        transform.GetComponent<Renderer>().material.color = initialColor;

        if (other.gameObject.CompareTag("Mesh"))
        {
            other.transform.parent.GetComponent<ModelingObject>().inTrashArea = false;
        }
    }
}
