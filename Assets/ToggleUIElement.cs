using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUIElement : MonoBehaviour {

	public bool active = false;

	public Image toggleBG;
	public RectTransform toggleObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Toggle(){
		active = !active;

		if (active) {
			toggleBG.gameObject.SetActive(true);
			//LeanTween.moveX (toggleObject, 20f, 0.1f).setEase (LeanTweenType.easeInOutQuad);
		} else {
			toggleBG.gameObject.SetActive(false);
			//LeanTween.moveX (toggleObject, 0f, 0.1f).setEase (LeanTweenType.easeInOutQuad);
		}
	}
}
