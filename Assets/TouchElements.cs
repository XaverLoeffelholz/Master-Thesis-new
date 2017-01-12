using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchElements : Singleton<TouchElements> {

	public CanvasGroup ScaleHandle;
	public CanvasGroup YMoveHandle;

	public CanvasGroup RotationToggle;
	public ToggleUIElement rotationToggleUIElement;

	public CanvasGroup TranslationToggle;

	public CanvasGroup RotateX;
	public CanvasGroup RotateY;
	public CanvasGroup RotateZ;
	public CanvasGroup bubble;

	public bool visible;
	public bool focused;
	public CanvasGroup allElememts;

	public Transform currentTrans;

	public Selection selectionManager;

	// Use this for initialization
	void Start () {
		visible = false;
		focused = false;
		allElememts.alpha = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentTrans != null) {
			transform.position = Camera.main.WorldToScreenPoint(currentTrans.position) + new Vector3(0f,25f,0f);

			if (rotationToggleUIElement.active && selectionManager.currentSelection != null) {				
				PositionRotationButtons (selectionManager.currentSelection.GetComponent<ModelingObject>());
			}
		}
	}

	public void Focus(){
		if (visible && allElememts.alpha >= 0.5f) {
			focused = true;
			//LeanTween.alphaCanvas (allElememts, 1f, 0.05f).setEase (LeanTweenType.easeInOutCubic);
		}
	}

	public void UnFocus(){
		if (visible && allElememts.alpha >= 0.5f) {
			focused = false;
			//LeanTween.alphaCanvas (allElememts, 0.5f, 0.05f).setEase (LeanTweenType.easeInOutCubic);
		}
	}

	public void Show (Transform obj, ModelingObject currentObj){
		currentTrans = obj;

		visible = true;
		allElememts.interactable = true;
		allElememts.blocksRaycasts = true;

		PositionRotationButtons(currentObj);

		bubble.alpha = 1f;
		ScaleHandle.alpha = 1f;
		YMoveHandle.alpha = 1f;
		RotationToggle.alpha = 1f;

		LeanTween.alphaCanvas (allElememts, 1f, 0.1f).setEase (LeanTweenType.easeInOutCubic);
	}

	public void Hide (){
		if (!focused) {
			LeanTween.alphaCanvas (allElememts, 0f, 0.01f);

			visible = false;
			allElememts.interactable = false;
			allElememts.blocksRaycasts = false;
		}	
	}

	public void HideButtonsExcept(handle visibleHandle){
		//CanvasGroup keepButton; 
		ShowRotationButtons (false);
		bubble.alpha = 0f;

		if (visibleHandle != null) {
			if (visibleHandle.typeOfHandle == handle.handleType.MoveY) {
				ScaleHandle.alpha = 0f;
				RotationToggle.alpha = 0f;
			} else if (visibleHandle.typeOfHandle == handle.handleType.UniformScale) {
				YMoveHandle.alpha = 0f;
				RotationToggle.alpha = 0f;
			} else if (visibleHandle.typeOfHandle == handle.handleType.RotationHandleToggle) {
				YMoveHandle.alpha = 0f;
				ScaleHandle.alpha = 0f;
				RotationToggle.alpha = 1f;
				bubble.alpha = 1f;
			}
		}
	}

	public void SetRotationToggleActive(){
		if (!rotationToggleUIElement.active) {
			rotationToggleUIElement.Toggle ();
		}

		ShowRotationButtons (true);
		bubble.alpha = 1f;
	}

	public void ShowRotationButtons(bool value){
		RotateX.gameObject.SetActive (value);
		RotateY.gameObject.SetActive (value);
		RotateZ.gameObject.SetActive (value);
	}

	public void SetRotationToggleInActive(){
		if (rotationToggleUIElement.active) {
			rotationToggleUIElement.Toggle ();
		}

		ShowRotationButtons (false);
	}

	public void PositionRotationButtons(ModelingObject currentSelectedObject){
		currentSelectedObject.CalculateBoundingBox ();
		Vector3 centerOfObject = currentSelectedObject.GetBoundingBoxCenter ();

		// create vector from object center to camera
		Vector3 cameraToObject = currentSelectedObject.GetPosOfClosestVertex(Camera.main.transform.position, currentSelectedObject.boundingBox.coordinates) - centerOfObject;

		// map vector on 3 planes of rotation circles
		Vector3 xVector = Vector3.ProjectOnPlane(cameraToObject, currentSelectedObject.handles.RotateX.transform.forward).normalized; 
		Vector3 yVector = Vector3.ProjectOnPlane(cameraToObject, currentSelectedObject.handles.RotateY.transform.forward).normalized; 
		Vector3 zVector = Vector3.ProjectOnPlane(cameraToObject, currentSelectedObject.handles.RotateZ.transform.forward).normalized; 

		// rotate vector so that it is at 22.5 degree
		float angleX = Vector3.Angle(xVector, currentSelectedObject.handles.RotateX.transform.up);
		float angleY = Vector3.Angle(yVector, currentSelectedObject.handles.RotateY.transform.up);
		float angleZ = Vector3.Angle(zVector, currentSelectedObject.handles.RotateZ.transform.up);

		float roundedAngleX = (Mathf.Round (angleX / 90f)) * 90f;
		float roundedAngleY = (Mathf.Round (angleY / 90f)) * 90f;
		float roundedAngleZ = (Mathf.Round (angleZ / 90f)) * 90f;

		//xVector = Quaternion.AngleAxis(roundedAngleX-angleX + 22.5f, currentSelectedObject.handles.RotateX.transform.forward) * xVector;
		//yVector = Quaternion.AngleAxis(roundedAngleY-angleY + 22.5f, currentSelectedObject.handles.RotateY.transform.forward) * yVector;
		//zVector = Quaternion.AngleAxis(roundedAngleZ-angleZ + 22.5f, currentSelectedObject.handles.RotateZ.transform.forward) * zVector;

		// place in distance of circle radius to center on mapped vector
		float distanceToCenter = currentSelectedObject.boundingBox.getMaxDistanceinBB(centerOfObject);

		Vector3 posXButton = currentSelectedObject.GetBoundingBoxCenter () + xVector * distanceToCenter + 0.1f * xVector;
		Vector3 posYButton = currentSelectedObject.GetBoundingBoxCenter () + yVector * distanceToCenter + 0.1f * yVector;
		Vector3 posZButton = currentSelectedObject.GetBoundingBoxCenter () + zVector * distanceToCenter + 0.1f * zVector;

		// convert 3D point to 2D and place rotation button
		RotateX.transform.position = Camera.main.WorldToScreenPoint(posXButton);
		RotateY.transform.position = Camera.main.WorldToScreenPoint(posYButton);
		RotateZ.transform.position = Camera.main.WorldToScreenPoint(posZButton);
	}

	public void UnfocusAll(){
		RotateX.gameObject.GetComponent<NewUielement> ().UnFocus ();	

		RotateY.gameObject.GetComponent<NewUielement> ().UnFocus ();
		RotateZ.gameObject.GetComponent<NewUielement> ().UnFocus ();

		ScaleHandle.gameObject.GetComponent<NewUielement> ().UnFocus ();
		YMoveHandle.gameObject.GetComponent<NewUielement> ().UnFocus ();
		RotationToggle.gameObject.GetComponent<NewUielement> ().UnFocus ();
	}
}
