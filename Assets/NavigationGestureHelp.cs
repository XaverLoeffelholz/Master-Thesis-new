﻿using UnityEngine;
using System.Collections;

public class NavigationGestureHelp : MonoBehaviour {

	public Selection controller1;
	public Selection controller2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position =  controller1.transform.position + (controller2.transform.position - controller1.transform.position).normalized * 0.15f;
		transform.position =  controller1.transform.position;
		transform.localRotation = controller1.transform.localRotation;
		//transform.LookAt (controller1.transform);
	}
}
