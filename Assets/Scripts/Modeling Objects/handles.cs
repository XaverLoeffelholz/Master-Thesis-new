using UnityEngine;
using System.Collections;

public class handles : MonoBehaviour {

    public GameObject faceTopScale;
    public GameObject faceBottomScale;
    public GameObject CenterTopPosition;
    public GameObject CenterBottomPosition;
    public GameObject HeightTop;
    public GameObject HeightBottom;
    public GameObject RotationX;
    public GameObject RotationY;
    public GameObject RotationZ;

    public GameObject RotationXStepwise;
    public GameObject RotationYStepwise;
    public GameObject RotationZStepwise;

    public GameObject circleTop;
    public GameObject circleBottom;

    public GameObject TopHandles;
    public GameObject BottomHandles;

    public bool objectFocused;

    // Use this for initialization
    void Start () {
        objectFocused = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisableHandles()
    {
        foreach (Transform handle in transform.GetChild(0)) {
            handle.gameObject.SetActive(false);
        }
    }

    public void ShowRotationHandles()
    {
        DisableHandles();
        RotationX.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationY.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationZ.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationX.SetActive(true);
        RotationY.SetActive(true);
        RotationZ.SetActive(true);

        RotationXStepwise.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationYStepwise.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationZStepwise.transform.localPosition = new Vector3(0f, 0f, 0f);
        RotationXStepwise.SetActive(true);
        RotationYStepwise.SetActive(true);
        RotationZStepwise.SetActive(true);
    }

    public void ShowFrustumHandles()
    {
        DisableHandles();
        transform.parent.GetComponent<ModelingObject>().PositionHandles();
        transform.parent.GetComponent<ModelingObject>().RotateHandles();

        TopHandles.SetActive(true);
        BottomHandles.SetActive(true);
        faceBottomScale.SetActive(true);
        faceTopScale.SetActive(true);
        HeightTop.SetActive(true);
        HeightBottom.SetActive(true);

        // change later
        CenterBottomPosition.SetActive(true);
        CenterTopPosition.SetActive(true);
    }

    public void ShowFrustumCenterHandles()
    {
        DisableHandles();
        transform.parent.GetComponent<ModelingObject>().PositionHandles();
        CenterBottomPosition.SetActive(true);
        CenterTopPosition.SetActive(true);
    }

}
