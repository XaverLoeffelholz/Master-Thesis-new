using UnityEngine;
using System.Collections;

public class RasterManager : Singleton<RasterManager> {

    public float rasterLevel;

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

    public int getNumberOfGridUnits(float value1, float value2)
    {
        int count = (int) (Mathf.Round(value1 / rasterLevel) - Mathf.Round(value2 / rasterLevel));
        return count;
    }

    public float Raster(float input)
    {
        float count = Mathf.Round(input/rasterLevel);
        float rasteredFloat = count * rasterLevel;
        return rasteredFloat;
    }

    // 
    public Vector3 Raster(Vector3 input)
    {
        float countx = Mathf.Round(input.x / rasterLevel);
        float county = Mathf.Round(input.y / rasterLevel);
        float countz  = Mathf.Round(input.z / rasterLevel);

        Vector3 rasteredVector = new Vector3(countx * rasterLevel, county * rasterLevel, countz * rasterLevel);

        return rasteredVector;
    }
}
