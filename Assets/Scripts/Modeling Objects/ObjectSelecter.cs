﻿using UnityEngine;
using System.Collections;
using System;

public class ObjectSelecter : MonoBehaviour {

    public ModelingObject connectedObject;
	public Group connectedGroup;
    private Vector3 initialScale;
    private float objectScale = 0.25f;
	public GameObject buttonGameObject;
	private Transform stageScaler;
	public GameObject Collider;

    public bool active;
	private Vector3 initialScaleButtonGO;


	// Use this for initialization
	void Start () {
        active = false;

        initialScale = transform.localScale;
		initialScaleButtonGO = buttonGameObject.transform.localScale;

		stageScaler = GameObject.Find ("StageScaler").transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (active)
        {
			RescaleButton ();
        }
    }

	public void RescaleButton(){
		RePosition (Camera.main.transform.position);

		Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
		float dist = Mathf.Abs(plane.GetDistanceToPoint(transform.position));
		transform.localScale = initialScale * (dist / Mathf.Max(stageScaler.localScale.x, 0.4f));

		transform.LookAt (Camera.main.transform);
	}

	public void ShowSelectionButton(Selection controller){	
		if (!active) {
			RePosition (Camera.main.transform.position);
			buttonGameObject.SetActive (true);
			active = true;
			Collider.SetActive (true);
			LeanTween.alpha (buttonGameObject, 1f, 0.15f);

			if (connectedObject != null) {
				connectedObject.ShowBoundingBox ();
				connectedObject.boundingBox.ActivateBoundingBoxCollider ();
			} else if (connectedGroup != null) {
				connectedGroup.DrawBoundingBox ();
				connectedGroup.boundingBox.ActivateBoundingBoxCollider ();
			}
		}
    }

	public void HideSelectionButton(){		
		if (active) {
			if (connectedObject != null) {
				connectedObject.HideBoundingBox ();
				connectedObject.boundingBox.DeActivateBoundingBoxCollider ();
			} else if (connectedGroup != null) {
				connectedGroup.boundingBox.ClearBoundingBox();
				connectedGroup.boundingBox.DeActivateBoundingBoxCollider ();
			}

			active = false;
			Collider.SetActive (false);
		}

		LeanTween.alpha (buttonGameObject, 0f, 0.2f);
    }
		
	public void Focus(Selection controller){
		if (active) {
			LeanTween.color(buttonGameObject, UiCanvasGroup.Instance.hoverColor, 0.2f).setEase(LeanTweenType.easeInOutCubic);
			LeanTween.scale(buttonGameObject, initialScaleButtonGO*1.2f, 0.2f).setEase(LeanTweenType.easeInOutCubic);
		}
	}

	public void FocusConnectedObject(Selection controller){
		if (connectedObject != null) {
			connectedObject.Focus (controller);
		} else if (connectedGroup != null) {
			connectedGroup.FocusGroup (null, controller);
		}
	}

	public void UnFocusConnectedObject(Selection controller){
		if (connectedObject != null) {
			connectedObject.UnFocus (controller);
		} else if (connectedGroup != null) {
			connectedGroup.UnFocusGroup (null, controller);
		}
	}

	public void UnFocus(Selection controller){
		if (active) {
			LeanTween.color (buttonGameObject, UiCanvasGroup.Instance.normalColor, 0.2f).setEase (LeanTweenType.easeInOutCubic);
			LeanTween.scale (buttonGameObject, initialScaleButtonGO, 0.2f).setEase (LeanTweenType.easeInOutCubic);
		}
	}

    public void Select(Selection controller, Vector3 uiPosition)
    {
		if (controller.currentSelection != null) {
			controller.currentSelection.GetComponent<ModelingObject> ().DeSelect (controller);
		}

		if (controller.otherController.currentSelection != null) {
			controller.otherController.currentSelection.GetComponent<ModelingObject> ().DeSelect (controller);
		}

		Collider.SetActive (false);

		if (connectedObject != null) {
			connectedObject.Select (controller, uiPosition);
			ObjectsManager.Instance.DisableObjectsExcept (connectedObject);
		} else if (connectedGroup != null) {
			connectedGroup.objectList[0].Select (controller, uiPosition);
			ObjectsManager.Instance.DisableObjectsExcept (connectedGroup);
		}
    }

	public void DeSelect(Selection controller)
	{
	    HideSelectionButton ();
	}

	public void RePosition(Vector3 position)
    {
		if (connectedObject != null) {
			transform.position = connectedObject.GetPosOfClosestVertex (position, new Vector3[] {connectedObject.boundingBox.coordinates[4], connectedObject.boundingBox.coordinates[5],connectedObject.boundingBox.coordinates[6],connectedObject.boundingBox.coordinates[7] });
		} else if (connectedGroup != null) {
			transform.position = connectedGroup.GetPosOfClosestVertex (position, new Vector3[] {connectedGroup.boundingBox.coordinates[4], connectedGroup.boundingBox.coordinates[5],connectedGroup.boundingBox.coordinates[6],connectedGroup.boundingBox.coordinates[7] });
		}
	}

	public void MoveAndFace (Vector3 position){
		DeactivateCollider ();

		LeanTween.color(buttonGameObject, UiCanvasGroup.Instance.clickColor, 0.01f).setEase(LeanTweenType.easeInOutCubic);
		LeanTween.color(buttonGameObject, UiCanvasGroup.Instance.normalColor, 0.01f).setEase(LeanTweenType.easeInOutCubic).setDelay(0.02f);

		LeanTween.alpha (buttonGameObject, 0f, 0.35f);
		LeanTween.scale (gameObject, gameObject.transform.localScale*2f, 0.4f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.move (gameObject, position, 0.4f).setEase (LeanTweenType.easeInOutCubic);
	}

	public void DeactivateCollider(){
		Collider.SetActive (false);
	}

}
