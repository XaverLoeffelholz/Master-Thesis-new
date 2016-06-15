using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiCanvasGroup : Singleton<UiCanvasGroup>{
    public bool visible;
    public GameObject player;
    private CanvasGroup canvGroup;

    public float positioningWidth;
    public float positioningHeight;
    public ModelingObject currentModelingObject;

    public GameObject MainMenu;

    // Use this for initialization
    void Start () {
        visible = false;
        canvGroup = this.GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        if (visible)
        {
            transform.LookAt(player.transform);
        }

    }

    public void OpenMainMenu(ModelingObject modelingObject)
    {
        currentModelingObject = modelingObject;
        MainMenu.GetComponent<UIMenu>().ActivateMenu();
    }

    public void Show()
    {
        LeanTween.alphaCanvas(canvGroup, 1f, 0.3f);
        visible = true;
    }

    public void Hide()
    {
        LeanTween.alphaCanvas(canvGroup, 0f, 0.3f);
        if(currentModelingObject!= null)
        {
            currentModelingObject.handles.DisableHandles();
        }
        visible = false;
    }

    public void ArrangeUIObjects(List<GameObject> elements)
    {
        // define lenght based on number of elements
        float width = elements.Count * positioningWidth;

        // define height based on number of elements
        float height = elements.Count * positioningHeight;

        // go through all elements

        for (int i=0; i< elements.Count; i++)
        {
            float x = (i * positioningWidth) - width / 2;

            float y = ((Mathf.Sin((float) i / (float) elements.Count * Mathf.PI)) * positioningHeight) - height / 2;
            elements[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x, y, 0f);
        }
    }
}
