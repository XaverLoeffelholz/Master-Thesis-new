﻿using UnityEngine;
using System.Collections;
using System;

public class ObjectSelecter : MonoBehaviour {

    public ModelingObject connectedObject;
    public Camera userCamera;
    private Vector3 initialScale;
    private float objectScale = 0.25f;
	public GameObject Highlighter;
	public GameObject buttonGameObject;

    public bool active;

	// Use this for initialization
	void Start () {
        active = false;

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
            transform.localScale = initialScale * dist * objectScale;

			transform.LookAt (userCamera.transform);
        }


    }

	public void ShowSelectionButton(Selection controller){
		RePosition (controller);
		active = true;
		buttonGameObject.SetActive (true);
    }

	public void HideSelectionButton(){
		active = false;
		buttonGameObject.SetActive (false);
    }

	public void Focus(Selection controller){
		
		// use for hover effects

	}

	public void UnFocus(Selection controller){

	}

    public void Select(Selection controller, Vector3 uiPosition)
    {
        connectedObject.Select(controller, uiPosition);
		Highlighter.SetActive (true);
    }

	public void DeSelect(Selection controller)
	{
	    HideSelectionButton ();
		Highlighter.SetActive (false);
	}

	public void RePosition(Selection controller)
    {
		transform.position = connectedObject.GetPosOfClosestVertex (controller.transform.position);
    }
}
