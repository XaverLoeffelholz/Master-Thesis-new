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

	private Color standardColor;
	private Color highlightedColor;

    public bool active;

	public GameObject Hover;

	// Use this for initialization
	void Start () {
        active = false;

        initialScale = transform.localScale;

        if (userCamera == null)
            userCamera = Camera.main;

		stageScaler = GameObject.Find ("StageScaler").transform;

		standardColor = buttonGameObject.GetComponent<Renderer> ().material.color;
		highlightedColor = new Color(standardColor.r * 0.5f, standardColor.g * 0.5f, standardColor.b * 0.5f, 1f);
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
		RePosition (controller);
		active = true;
		buttonGameObject.SetActive (true);
    }

	public void HideSelectionButton(){
		active = false;
		buttonGameObject.SetActive (false);
    }

	public void Focus(Selection controller){
		LeanTween.alpha(Hover, 1f, 0.3f).setEase(LeanTweenType.easeInOutCubic);
		//LeanTween.scale(buttonGameObject, transform.lossyScale *1.2f, 0.3f).setEase(LeanTweenType.easeInOutCubic);
		//LeanTween.color(buttonGameObject, highlightedColor, 0.3f).setEase(LeanTweenType.easeInOutCubic);
	}

	public void UnFocus(Selection controller){
		LeanTween.alpha(Hover, 0f, 0.3f).setEase(LeanTweenType.easeInOutCubic);

		//LeanTween.scale(buttonGameObject, transform.lossyScale/1.2f, 0.3f).setEase(LeanTweenType.easeInOutCubic);
		//LeanTween.color(buttonGameObject, standardColor, 0.3f).setEase(LeanTweenType.easeInOutCubic);
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
}
