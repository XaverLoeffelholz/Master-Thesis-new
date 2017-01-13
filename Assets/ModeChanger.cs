using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeChanger : Singleton<ModeChanger> {

	public bool modeChangerActive = true;

	public enum BuildingMode { RotationMode, BoundingBoxMode }
	public BuildingMode currentMode;

	public Image ButtonLeftActive;
	public Image ButtonLeftInActive;
	public Image ButtonRightActive;
	public Image ButtonRightInActive;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMode(BuildingMode newMode){
		currentMode = newMode;
	}

	public void ActiveRotationMode(){
		SetMode (BuildingMode.RotationMode);
		ButtonLeftActive.color = Color.white;
		ButtonLeftInActive.color = Color.clear;
		ButtonRightActive.color = Color.clear;
		ButtonRightInActive.color = Color.white;
	}

	public void BoundingBoxMode(){
		SetMode (BuildingMode.BoundingBoxMode);
		ButtonLeftActive.color = Color.clear;
		ButtonLeftInActive.color = Color.white;
		ButtonRightActive.color = Color.white;
		ButtonRightInActive.color = Color.clear;
	}
}
