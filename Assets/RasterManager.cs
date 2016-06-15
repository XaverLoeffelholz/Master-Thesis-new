using UnityEngine;
using System.Collections;

public class RasterManager : Singleton<RasterManager> {

    public float rasterLevel = 0.001f;

    public float max_X;
    public float min_X;

    public float max_Y;
    public float min_Y;

    public float max_Z;
    public float min_Z;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public float Raster(float input)
    {
        double count = System.Math.Round((input/rasterLevel), 1);
        float rasteredFloat = (float) (count * rasterLevel);
        return input;
    }

    // 
    public Vector3 Raster(Vector3 input)
    {
        float countx = Mathf.Round(input.x / rasterLevel);
        float county = Mathf.Round(input.y / rasterLevel);
        float countz  = Mathf.Round(input.z / rasterLevel);

        Vector3 rasteredVector = new Vector3(countx * rasterLevel, county * rasterLevel, countz * rasterLevel);

        /*

        if (input.x > max_X)
        {
            input.x = max_X;
        }
        else if (input.x < min_X)
        {
            input.x = min_X;
        }

        if (input.y > max_Y)
        {
            input.y = max_Y;
        }
        else if (input.y < min_Y)
        {
            input.y = min_Y;
        }

        if (input.z > max_Z)
        {
            input.z = max_Z;
        }
        else if (input.z < min_Z)
        {
            input.z = min_Z;
        }

        */

        return input;
    }
}
