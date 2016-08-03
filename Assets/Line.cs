using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

    public GameObject p1;
    public GameObject p2;
    public GameObject line;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerBetween = p1.transform.position * 0.5f + p2.transform.position * 0.5f;
        line.transform.position = centerBetween;

        Quaternion newRotation = Quaternion.LookRotation(p1.transform.position - p2.transform.position);
        line.transform.localRotation = newRotation;

        line.transform.localScale = new Vector3(line.transform.localScale.x, line.transform.localScale.y, (p1.transform.position - p2.transform.position).magnitude + line.transform.localScale.x);
    }

    public void DrawLineWorldCoord(Vector3 pos1, Vector3 pos2)
    {
        p1.transform.position = pos1;
        p2.transform.position = pos2;
    }

    public void DrawLineLocalCoord(Vector3 pos1, Vector3 pos2)
    {
        p1.transform.localPosition = pos1;
        p2.transform.localPosition = pos2;
    }
}
