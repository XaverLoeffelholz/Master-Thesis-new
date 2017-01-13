using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBubble : MonoBehaviour {

	public CanvasGroup bubble;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show(){
		LeanTween.alphaCanvas (bubble, 1f, 0.15f).setEase (LeanTweenType.easeInOutExpo);
	}

	public void Hide(){
		LeanTween.alphaCanvas (bubble, 0f, 0.15f).setEase (LeanTweenType.easeInOutExpo);
	}
}
