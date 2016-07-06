using UnityEngine;
using System.Collections;
using System;

public class ObjectSelecter : MonoBehaviour {

    public ModelingObject connectedObject;
    public Camera userCamera;
    private Vector3 initialScale;
    public float objectScale = 0.03f;

    public bool active;

	// Use this for initialization
	void Start () {
        active = true;

        initialScale = transform.localScale;

        if (userCamera == null)
            userCamera = Camera.main;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (active)
        {
            Plane plane = new Plane(userCamera.transform.forward, userCamera.transform.position);
            float dist = plane.GetDistanceToPoint(transform.position);
            transform.localScale = initialScale * Mathf.Sqrt(dist) * objectScale;
        }
    }

    public void Focus(Selection controller)
    {
        connectedObject.Focus(controller);
    }

    public void UnFocus()
    {

    }

    public void Select(Selection controller, Vector3 uiPosition)
    {
        connectedObject.Select(controller, uiPosition);
    }

    public void ReScale()
    {



    }
}
