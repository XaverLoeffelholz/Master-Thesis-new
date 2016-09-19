using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class Logger : MonoBehaviour {

    public enum typeOfLog { triggerNoTarget, touchpadLeft, touchpadRight, nonUniformScaleHandle, RotationHandle, FrustumHandle, stage, uiElement };
    public enum generalType { touchpad, triggerInteraction };


    private string filePath = @"\";
    public string UserID;
    private string fileName;
    private FileStream fs;

    private float countTimeFrom;

    public StageFreeMovement stageMovement;
    public StageController stageController;
    public Selection selection;

    // Use this for initialization
    void Start () {
        countTimeFrom = Time.time;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RestartTimer()
    {
        countTimeFrom = Time.time;
    }

    public void CreateTextFile()
    {
        fs = File.Create(filePath);        
    }

    public void AddLine(Logger.typeOfLog logtype)
    {
        generalType currentType;

        if (logtype == typeOfLog.touchpadLeft || logtype == typeOfLog.touchpadRight)
        {

            currentType = generalType.touchpad; 
        } else
        {
            currentType = generalType.triggerInteraction;
        }

       string text = UserID + "," + currentType + "," + (Time.time - countTimeFrom) + "," + logtype + "," + selection.currentSettingSelectionMode + "," + stageMovement.currentStageMovement + "," + stageController.currentRotationScalingTechnique + ;
       File.AppendAllText(filePath, text);
    }
}
