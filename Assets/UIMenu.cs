using UnityEngine;
using UnityEngine.UI;
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

    public enum buttonType
    {
        Menu,
        Delete,
        Group,
        Duplicate,
        Color,
        ResetFrustum,
        ResetRotation,
        ResetColor
    }

    public menuType TypeOfMenu;
    public Selection controller1;
    public Selection controller2;
    public ColorPicker colorPicker;

    private UiCanvasGroup parentCanvas;

    // Use this for initialization
    void Start () {
        parentCanvas = transform.parent.GetComponent<UiCanvasGroup>();
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
            if (child.parent == transform && !child.CompareTag("Colors"))
            {
                child.gameObject.SetActive(true);
                buttons.Add(child.gameObject);
            }

        }
        transform.parent.GetComponent<UiCanvasGroup>().ArrangeUIObjects(buttons);
        transform.parent.GetComponent<UiCanvasGroup>().Show();

        controller1.enableFaceSelection(false);
        controller2.enableFaceSelection(false);
        colorPicker.gameObject.SetActive(false);


        // Show the right handles
        switch (TypeOfMenu)
        {
            case (menuType.Rotation):
                parentCanvas.currentModelingObject.handles.ShowRotationHandles();
                break;
            case (menuType.NonUniformScaling):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Color):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                colorPicker.gameObject.SetActive(true);
                break;
            case (menuType.Shape):
                parentCanvas.currentModelingObject.handles.ShowFrustumHandles();
                break;
            case (menuType.Object):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
		    case (menuType.NewObject):
                parentCanvas.currentModelingObject.handles.DisableHandles ();
				controller1.enableFaceSelection (true);
                controller2.enableFaceSelection(true);
                break;
            case (menuType.MainMenu):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
        }
    }

    public void PerformAction(UiElement button)
    {
        switch (button.typeOfButton)
        {
            case (buttonType.Menu):
                break;
            case (buttonType.Color):
                ChangeColor(button);
                break;
            case (buttonType.Delete):
                Delete();
                break;
            case (buttonType.Duplicate):
                Duplicate();
                break;
            case (buttonType.Group):
                Group();
                break;
            case (buttonType.ResetColor):
                break;
            case (buttonType.ResetFrustum):
                break;
            case (buttonType.ResetRotation):
                break;
        }
    }


    // All functions by buttons

    public void Group()
    {

    }

    public void Delete()
    {
        parentCanvas.Hide();
        UiCanvasGroup.Instance.currentModelingObject.Trash();
    }

    public void Duplicate()
    {

    }

    public void ChangeColor(UiElement button)
    {
        UiCanvasGroup.Instance.currentModelingObject.ChangeColor(button.gameObject.GetComponent<Image>().color);
    }
}
