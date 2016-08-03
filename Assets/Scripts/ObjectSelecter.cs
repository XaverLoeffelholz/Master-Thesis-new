using UnityEngine;
using System.Collections;
using System;

public class ObjectSelecter : MonoBehaviour {

    public ModelingObject connectedObject;
    public Camera userCamera;
    private Vector3 initialScale;
    private float objectScale = 0.25f;
	public GameObject Highlighter;
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

        if (userCamera == null)
            userCamera = Camera.main;

		stageScaler = GameObject.Find ("StageScaler").transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (active)
        {
            Plane plane = new Plane(userCamera.transform.forward, userCamera.transform.position);
			float dist = Mathf.Abs(plane.GetDistanceToPoint(transform.position));
			transform.localScale = initialScale * (Mathf.Sqrt(dist) / stageScaler.localScale.x);

			if (stageScaler.localScale.x < 0.5f) {
				transform.localScale = transform.localScale * 0.5f;
			}

			transform.LookAt (userCamera.transform);
        }


    }

	public void ShowSelectionButton(Selection controller){
		if (!active) {
			buttonGameObject.SetActive (true);
			RePosition (controller);
			active = true;
			Collider.SetActive (true);
		}

		LeanTween.alpha (buttonGameObject, 1f, 0.15f);
    }

	public void HideSelectionButton(){
		if (active) {
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
		transform.position = connectedObject.GetPosOfClosestVertex (controller.transform.position, Face.faceType.BottomFace);
    }

	public void MoveAndFace (Vector3 position){
		DeactivateCollider ();

		LeanTween.color(buttonGameObject, UiCanvasGroup.Instance.clickColor, 0.01f).setEase(LeanTweenType.easeInOutCubic);
		LeanTween.color(buttonGameObject, UiCanvasGroup.Instance.normalColor, 0.01f).setEase(LeanTweenType.easeInOutCubic).setDelay(0.02f);

		LeanTween.alpha (buttonGameObject, 0f, 0.35f);
		LeanTween.move (gameObject, position, 0.4f).setEase (LeanTweenType.easeInOutCubic);
	}

	public void DeactivateCollider(){
		Collider.SetActive (false);
	}

}
