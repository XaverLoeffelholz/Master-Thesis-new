using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenu : MonoBehaviour {

    public enum menuType
    {
        MainMenu,
        Rotation,
        NonUniformScaling,
        Color,
        Shape,
        Object,
        NewObject
    }

    public menuType TypeOfMenu;
    public Selection controller1;
    public Selection controller2;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivateMenu()
    {
        // Hide the menu
        transform.parent.GetComponent<UiCanvasGroup>().Hide();

        // get all menus, deactivate them
        List<GameObject> UiMenus = new List<GameObject>();

        // only activate this one
        foreach (Transform menu in transform.parent)
        {
            if (menu != this.gameObject)
            {
                // deactivate all buttons in other menus
                foreach (Transform button in menu)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        List<GameObject> buttons = new List<GameObject>();
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            buttons.Add(child.gameObject);
        }
        transform.parent.GetComponent<UiCanvasGroup>().ArrangeUIObjects(buttons);
        transform.parent.GetComponent<UiCanvasGroup>().Show();

        controller1.enableFaceSelection(false);
        controller2.enableFaceSelection(false);

        // Show the right handles
        switch (TypeOfMenu)
        {
            case (menuType.Rotation):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.ShowRotationHandles();
                break;
            case (menuType.NonUniformScaling):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Color):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Shape):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.ShowFrustumHandles();
                break;
            case (menuType.Object):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.DisableHandles();
                break;
		case (menuType.NewObject):
				transform.parent.GetComponent<UiCanvasGroup> ().currentModelingObject.handles.DisableHandles ();
				controller1.enableFaceSelection (true);
                controller2.enableFaceSelection(true);
                break;
            case (menuType.MainMenu):
                transform.parent.GetComponent<UiCanvasGroup>().currentModelingObject.handles.DisableHandles();
                break;
        }
    }
}
