using UnityEngine;
using System.Collections;

public class Teleportation : Singleton<Teleportation> {

    public Transform View1;
    public Transform View2;
    public Transform View3;
    public Transform View4;
	public Transform MovingObject;

    int i = 1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            i++;

            if (i < 4)
            {
                JumpToPos(i);
            }
        }

    }

    public void JumpToPos(int pos)
    {
        switch (pos)
        {
            case 1:
                transform.position = View1.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
                break;
            case 2:
                transform.position = View2.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                break;
            case 3:
                transform.position = View3.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                break;
            case 4:
                transform.position = View4.position;
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                break;
			case 5:
				transform.position = MovingObject.position ;
				transform.rotation = MovingObject.rotation;
				break;
        }
    }
}
