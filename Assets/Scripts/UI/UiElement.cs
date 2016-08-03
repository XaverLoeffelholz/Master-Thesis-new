﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiElement : MonoBehaviour {

    public RectTransform title;
    public UIMenu goal;
	public bool focused;
    public UIMenu.buttonType typeOfButton;


	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void Focus(Selection controller)
    {
		if (!focused) {
            controller.AssignCurrentFocus(transform.gameObject);
			LeanTween.alphaText(title, 1.0f, 0.2f);
			LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.3f);

			if (typeOfButton != UIMenu.buttonType.CloseMenu && typeOfButton != UIMenu.buttonType.Color) {
				LeanTween.color (this.GetComponent<RectTransform> (), UiCanvasGroup.Instance.hoverColor, 0.2f);
			}

			// maybe not so fast
			foreach (Transform button in transform.parent)
			{
				if (button != transform)
				{
					button.gameObject.GetComponent<UiElement>().UnFocus(controller);
				}
			}

			focused = true;
		}
     
    }

    public void UnFocus(Selection controller)
    {
		if (focused) {
            controller.DeAssignCurrentFocus(transform.gameObject);
			LeanTween.alphaText(title, 0.0f, 0.2f);
			LeanTween.scale(this.gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.3f);

			if (typeOfButton != UIMenu.buttonType.CloseMenu && typeOfButton != UIMenu.buttonType.Color) {
				LeanTween.color (this.GetComponent<RectTransform> (), UiCanvasGroup.Instance.normalColor, 0.2f);
			}

			focused = false;
		}
    }

    public void PerformAction(Selection controller)
    { 
		if (typeOfButton != UIMenu.buttonType.CloseMenu && typeOfButton != UIMenu.buttonType.Color) {
			LeanTween.color (this.GetComponent<RectTransform> (), UiCanvasGroup.Instance.clickColor, 0.02f);
			LeanTween.color (this.GetComponent<RectTransform> (), UiCanvasGroup.Instance.normalColor, 0.02f).setDelay(0.02f);
		}		

        transform.parent.GetComponent<UIMenu>().PerformAction(this, controller);
    }

}
