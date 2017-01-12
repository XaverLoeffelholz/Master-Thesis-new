using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeAngle : Singleton<VisualizeAngle> {

	public CanvasGroup bubble;
	public Text angleText;


	// Use this for initialization
	void Start () {
		Hide ();
	}

	public void Show(){
		bubble.alpha = 1f;
	}

	public void Hide(){
		bubble.alpha = 0f;
	}

	public void SetNumber(float angle){
		angleText.text = "" + angle.ToString() + "° ";
	}

	// Update is called once per frame
	void Update () {
		transform.position = Input.mousePosition;
	}
}
