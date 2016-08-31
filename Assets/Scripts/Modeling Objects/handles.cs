using UnityEngine;
using System.Collections;

public class handles : MonoBehaviour {

    public GameObject faceTopScale;
    public GameObject faceBottomScale;
    public GameObject CenterTopPosition;
    public GameObject CenterBottomPosition;
    public GameObject HeightTop;
    public GameObject HeightBottom;

    public Transform RotationHandles;

    public GameObject RotateUp0;
    public GameObject RotateUp1;
    public GameObject RotateUp2;
    public GameObject RotateUp3;

    public GameObject RotateDown0;
    public GameObject RotateDown1;
    public GameObject RotateDown2;
    public GameObject RotateDown3;

    public GameObject RotateSide0;
    public GameObject RotateSide1;
    public GameObject RotateSide2;
    public GameObject RotateSide3;

    public GameObject TopHandles;
    public GameObject BottomHandles;

	public GameObject NonUniformScalingHandles;
	public GameObject NonUniformScaleFront;
	public GameObject NonUniformScaleBack;
	public GameObject NonUniformScaleTop;
	public GameObject NonUniformScaleBottom;
	public GameObject NonUniformScaleLeft;
	public GameObject NonUniformScaleRight;

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

        foreach (Transform handle in RotationHandles)
        {
            handle.gameObject.SetActive(false);
        }
    }

    public void ShowRotationHandles()
    {
        DisableHandles();
		transform.parent.GetComponent<ModelingObject>().PositionHandles(true);
		//transform.parent.GetComponent<ModelingObject>().RotateHandles();
    }

	public void HideRotationHandlesExcept(handle certainHandle){
		foreach (Transform handle in RotationHandles)
		{
			if (certainHandle == null || handle != certainHandle.transform) {
				handle.gameObject.SetActive(false);
			}
		}
	}

	public void HideScalingHandlesExcept(handle certainHandle){
		foreach (Transform handle in NonUniformScalingHandles.transform)
		{
			if (certainHandle == null || handle != certainHandle.transform) {
				handle.gameObject.SetActive(false);
			}
		}
	}

	public void ShowNonUniformScalingHandles() {
		DisableHandles();
		ShowRotationHandles ();

		NonUniformScalingHandles.SetActive (true);
		NonUniformScaleFront.SetActive(true);
		NonUniformScaleBack.SetActive(true);
		NonUniformScaleTop.SetActive(true);
		NonUniformScaleBottom.SetActive(true);
		NonUniformScaleLeft.SetActive(true);
		NonUniformScaleRight.SetActive(true);
	}

    public void ShowFrustumHandles() {
		
        DisableHandles();
        //transform.parent.GetComponent<ModelingObject>().PositionHandles();
        //transform.parent.GetComponent<ModelingObject>().RotateHandles();

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
        transform.parent.GetComponent<ModelingObject>().PositionHandles(false);
        CenterBottomPosition.SetActive(true);
        CenterTopPosition.SetActive(true);
    }

}
