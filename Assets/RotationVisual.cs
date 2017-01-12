using UnityEngine;
using System.Collections;

public class RotationVisual : MonoBehaviour {

	public GameObject firstSlice;
	public GameObject bigSlice;
    public Transform SlicesContainer;

	public GameObject RightangleVisual;
	public GameObject rightAngleSlice;

	public GameObject RotationPlane;
	public GameObject RotationPlaneBack;

	public GameObject RotationPlaneOuter;
	public GameObject RotationPlaneBackOuter;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
      
    }

	public void ColorCircle(Color color){
		
		RotationPlane.GetComponent<Renderer> ().material.color = color;
		RotationPlaneBack.GetComponent<Renderer> ().material.color = color;

		RotationPlaneOuter.GetComponent<Renderer> ().material.color = color;
		RotationPlaneBackOuter.GetComponent<Renderer> ().material.color = color;
	
	}

	public void RotationVisualisation(Vector3 point1, Vector3 point2, bool smooth, float angle)
    {
        float startAngle = Vector3.Angle(transform.up, point1 - transform.position);
        float directionStart = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point1 - transform.position), transform.forward));

        float endangle = Vector3.Angle(transform.up, point2 - transform.position);
        float directionEnd = Mathf.Sign(Vector3.Dot(Vector3.Cross(transform.up, point2 - transform.position), transform.forward));
       
		if (smooth) {
			if (!RotationPlaneBackOuter.active) {
				RotationPlane.SetActive (false);
				RotationPlaneBack.SetActive (false);
				RotationPlaneOuter.SetActive (true);
				RotationPlaneBackOuter.SetActive (true);
			}
			//CreateSlices(RasterManager.Instance.RasterAngleSmooth(startAngle * directionStart), RasterManager.Instance.RasterAngleSmooth(endangle * directionEnd), smooth);
		} else {
			if (!RotationPlane.activeSelf) {
				RotationPlane.SetActive (true);
				RotationPlaneBack.SetActive (true);
				RotationPlaneOuter.SetActive (false);
				RotationPlaneBackOuter.SetActive (false);
			}
			//CreateSlices(RasterManager.Instance.RasterAngle(startAngle * directionStart), RasterManager.Instance.RasterAngle(endangle * directionEnd), smooth);
		}

		float offSet = Mathf.Round(startAngle / 90f) * 90f  * directionStart;

		//Debug.Log ("Offset is " + offSet);

		CreateSlicesFromFixedPos (angle, smooth, offSet);

    }

	public void ShowRightAngleVisual(bool value){
		RightangleVisual.SetActive (value);
	}

	public void CreateSlices(float startAngle, float endangle, bool smooth){

        foreach(Transform child in SlicesContainer)
        {
            Destroy(child.gameObject);
        }

        if (startAngle < 0)
        {
            startAngle += 360;
        }

        if (endangle < 0)
        {
            endangle += 360;
        }

        float smallerValue;
        float biggerValue;

        if (startAngle > endangle)
        {
            smallerValue = endangle;
            biggerValue = startAngle;
        } else
        {
            smallerValue = startAngle;
            biggerValue = endangle;
        }

        float distance = biggerValue - smallerValue;

        if (distance > 180f)
        {
            startAngle = biggerValue;
            endangle = smallerValue;
            distance = 360-distance;
        } else
        {
            startAngle = smallerValue;
            endangle = biggerValue;
        }     

		int numberOfSlices = 0;

		if (smooth) {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(distance / RasterManager.Instance.rasterLevelAnglesSmooth));
		} else {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(distance / RasterManager.Instance.rasterLevelAngles));
		}


        for (int i=0; i < numberOfSlices; i++)
        {	
			GameObject newSlice;
			
			if (smooth) {
				newSlice = Instantiate (firstSlice);
			} else {
				newSlice = Instantiate (bigSlice);
			}

            newSlice.transform.position = transform.position;
            newSlice.transform.localEulerAngles = transform.localEulerAngles;

			if (smooth) {
				//newSlice.transform.Rotate(new Vector3(0f, 0f, 0f + i * RasterManager.Instance.rasterLevelAnglesSmooth));
				newSlice.transform.Rotate(new Vector3(0f, 0f, startAngle + i * RasterManager.Instance.rasterLevelAnglesSmooth));
			} else {
				//newSlice.transform.Rotate(new Vector3(0f, 0f, 0f + i * RasterManager.Instance.rasterLevelAngles));
				newSlice.transform.Rotate(new Vector3(0f, 0f, startAngle + i * RasterManager.Instance.rasterLevelAngles));
			}


			newSlice.transform.SetParent(SlicesContainer);
			newSlice.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);

			if (((float) i + 1f) % 4f == 0f && !smooth && numberOfSlices % 4f == 0f) {

				GameObject rightAngleSliceInstance;

				rightAngleSliceInstance = Instantiate (rightAngleSlice);
				rightAngleSliceInstance.transform.position = transform.position;
				rightAngleSliceInstance.transform.localEulerAngles = transform.localEulerAngles;

				rightAngleSliceInstance.transform.Rotate(new Vector3(0f, 0f, startAngle + (((i+1)-4f)/4f) * 90f));
				//rightAngleSliceInstance.transform.Rotate(new Vector3(0f, 0f, 0f + (((i+1)-4f)/4f) * 90f));

				rightAngleSliceInstance.transform.SetParent(SlicesContainer);
				rightAngleSliceInstance.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);
			}
        }

     
    }

	public void CreateSlicesFromFixedPos(float angle, bool smooth, float offsetAngle){

		foreach(Transform child in SlicesContainer)
		{
			Destroy(child.gameObject);
		}


		int numberOfSlices = 0;

		if (smooth) {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(angle / RasterManager.Instance.rasterLevelAnglesSmooth));
		} else {
			numberOfSlices = Mathf.Abs(Mathf.RoundToInt(angle / RasterManager.Instance.rasterLevelAngles));
		}


		for (int i=0; i < numberOfSlices; i++)
		{	
			GameObject newSlice;

			if (smooth) {
				newSlice = Instantiate (firstSlice);
			} else {
				newSlice = Instantiate (bigSlice);
			}

			newSlice.transform.position = transform.position;
			newSlice.transform.localEulerAngles = transform.localEulerAngles;

			if (smooth) {
				if (Mathf.Sign (angle) == 1f) {
					newSlice.transform.Rotate(new Vector3(0f, 0f, offsetAngle + i * RasterManager.Instance.rasterLevelAnglesSmooth));
				} else {
					newSlice.transform.Rotate(new Vector3(0f, 0f, offsetAngle + (i+1) * Mathf.Sign(angle) * RasterManager.Instance.rasterLevelAnglesSmooth));
				}
			} else {
				if (Mathf.Sign (angle) == 1f) {
					newSlice.transform.Rotate(new Vector3(0f, 0f, offsetAngle + i * RasterManager.Instance.rasterLevelAngles));
				} else {
					newSlice.transform.Rotate(new Vector3(0f, 0f, offsetAngle + (i+1) * Mathf.Sign(angle) * RasterManager.Instance.rasterLevelAngles));
				}
			}

			newSlice.transform.SetParent(SlicesContainer);
			newSlice.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);

			if (((float) i + 1f) % 4f == 0f && !smooth && numberOfSlices % 4f == 0f) {

				GameObject rightAngleSliceInstance;

				rightAngleSliceInstance = Instantiate (rightAngleSlice);
				rightAngleSliceInstance.transform.position = transform.position;
				rightAngleSliceInstance.transform.localEulerAngles = transform.localEulerAngles;

				if (Mathf.Sign (angle) == 1f) {
					rightAngleSliceInstance.transform.Rotate(new Vector3(0f, 0f, offsetAngle + (((i+1)-4f)/4f) * 90f * Mathf.Sign(angle)));
				} else {
					rightAngleSliceInstance.transform.Rotate(new Vector3(0f, 0f, offsetAngle + ((((i+1)-4f)/4f)+1) * 90f * Mathf.Sign(angle)));
				}


				rightAngleSliceInstance.transform.SetParent(SlicesContainer);
				rightAngleSliceInstance.transform.localScale = new Vector3 (0.15f,0.15f,0.15f);
			}
		}


	}
}
