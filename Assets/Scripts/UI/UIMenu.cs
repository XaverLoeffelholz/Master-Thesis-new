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
        Group,
        NewObject
    }

    public enum buttonType
    {
        Menu,
		CloseMenu,
        Delete,
        GroupStart,
        GroupEnd,
        Duplicate,
        Color,
        ResetFrustum,
        ResetRotation,
        ResetColor,
        Extrude
    }

    public menuType TypeOfMenu;
    public Selection controller1;
    public Selection controller2;

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
       // transform.parent.GetComponent<UiCanvasGroup>().Hide();

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
			if (child.parent == transform)
            {
				if (child.name == "UI Close"){
					child.gameObject.SetActive(true);
					child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(500f, -50f, 0f);
				} else {
					if(!child.gameObject.CompareTag("UIElementNotInUse")){
						child.gameObject.SetActive(true);
						buttons.Add(child.gameObject);
					}
				}
            }
        }
        transform.parent.GetComponent<UiCanvasGroup>().ArrangeUIObjects(buttons);
        transform.parent.GetComponent<UiCanvasGroup>().Show();

        controller1.enableFaceSelection(false);
        controller2.enableFaceSelection(false);

        // Show the right handles
        switch (TypeOfMenu)
        {
			case (menuType.Rotation):
				parentCanvas.currentModelingObject.PositionHandles ();
                parentCanvas.currentModelingObject.handles.ShowRotationHandles();
                parentCanvas.currentModelingObject.ShowBoundingBox();
                break;
            case (menuType.NonUniformScaling):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Color):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Shape):
                parentCanvas.currentModelingObject.handles.ShowFrustumHandles();
                break;
            case (menuType.Object):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.Group):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
            case (menuType.NewObject):
                parentCanvas.currentModelingObject.handles.DisableHandles ();
                break;
            case (menuType.MainMenu):
                parentCanvas.currentModelingObject.handles.DisableHandles();
                break;
        }
    }

    public void PerformAction(UiElement button, Selection controller)
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
				Duplicate ();

				//UiCanvasGroup.Instance.Hide ();
                //controller.SelectLatestObject();
                //parentCanvas.objectMenu.ActivateMenu();
                break;
            case (buttonType.GroupStart):
                StartGroup();
                break;
            case (buttonType.GroupEnd):
                EndGroup();
                break;
            case (buttonType.ResetColor):
                break;
            case (buttonType.ResetFrustum):
                break;
            case (buttonType.ResetRotation):
                break;
            case (buttonType.Extrude):
                Extrude();
                break;
			case (buttonType.CloseMenu):
				parentCanvas.CloseMenu(controller);
				break;
        }

    }


    // All functions by buttons
    public void Extrude()
    {
        controller1.enableFaceSelection(true);
        controller2.enableFaceSelection(true);
    }


    public void StartGroup()
    {
        // create group
        Group newGroup;
        newGroup = ObjectsManager.Instance.CreateGroup();

        // add first element to group
        ObjectsManager.Instance.AddObjectToGroup(newGroup, parentCanvas.currentModelingObject);

        // activate selection of item for group
        controller1.groupItemSelection = true;
        controller2.groupItemSelection = true;
    }



    public void EndGroup()
    {
        // deactivate selection of item for group
        controller1.groupItemSelection = false;
        controller2.groupItemSelection = false;
    }

    public void Delete()
    {
        parentCanvas.Hide();
        UiCanvasGroup.Instance.currentModelingObject.TrashObject(true);
    }

    public void Duplicate()
    {
        ObjectCreator.Instance.DuplicateObject(parentCanvas.currentModelingObject, null);
    }

    public void ChangeColor(UiElement button)
    {
        UiCanvasGroup.Instance.currentModelingObject.ChangeColor(button.gameObject.GetComponent<Image>().color, true);
    }
}
