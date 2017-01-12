using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUielement : MonoBehaviour {

	public enum typeOfUiElement { UniformScale, YMovement, ToggleRotation, RotateX, RotateY, RotateZ };
	public typeOfUiElement typeOfThis;

	public Selection SelectionManager;

	public Vector3 hoveredScale;
	public Vector3 normalScale;
	RectTransform wholeButton;

	// Use this for initialization
	void Start () {
		wholeButton = gameObject.GetComponent<RectTransform> ();
		normalScale = wholeButton.localScale;
		hoveredScale = normalScale * 1.1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Focus(){
	//	Debug.Log ("Focus " + gameObject.name);
		SelectionManager.currentlyFocusedUiElement = this;

		//if (typeOfThis != typeOfUiElement.ToggleRotation) {
			LeanTween.scale (wholeButton, hoveredScale, 0.08f).setEase (LeanTweenType.easeInOutQuad);
		//}
	}

	public void UnFocus(){
	//	Debug.Log ("UnFocus " + gameObject.name);
		SelectionManager.currentlyFocusedUiElement = null;
		if (wholeButton != null) {
			LeanTween.scale (wholeButton, normalScale, 0.08f).setEase (LeanTweenType.easeInOutQuad);
		}

		if (TouchManager.Instance.touchActive) {
			maxCamera.Instance.block = false;
		}
	}

	public void SelectForTouch(){
		if (TouchManager.Instance.touchActive) {
			SelectionManager.currentlyFocusedUiElement = this;
			maxCamera.Instance.block = true;
		}
	}


}
