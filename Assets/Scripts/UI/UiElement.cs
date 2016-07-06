using UnityEngine;
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
			focused = false;
		}
    }

    public void PerformAction(Selection controller)
    { 
        transform.parent.GetComponent<UIMenu>().PerformAction(this, controller);
    }

}
