using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class Logger : Singleton<Logger> {

	public enum typeOfLog { triggerOnObject, triggerNoTarget, touchpadScaleStage, touchpadRotateStage, touchpadMoveObject, touchpadRotateObject, nonUniformScaleHandle, RotationHandle, FrustumHandle, stage, uiElement };
    public enum generalType { touchpad, triggerInteraction };

    private string filePath = @"C:\Users\user\Documents\MasterThesis Xaver - new Repo\";
    public string UserID;
    private FileStream fs;

    private float countTimeFrom;

    public StageFreeMovement stageMovement;
    public StageController stageController;
    public Selection selection;
	public int sessionNumber;

    // Use this for initialization
    void Start () {

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
		filePath += "LogUser" + UserID + "_session" + sessionNumber + ".csv";

		RestartTimer ();
		fs = File.Create(filePath);  
		fs.Dispose ();
	 	
		String header = "UserID, General Type of Interaction, Time, Specific Type of Interaction, Settings Selection Mode, Stage movement, stage rotation and scaling, session number \n";
		File.AppendAllText(filePath, header);
    }

    public void AddLine(Logger.typeOfLog logtype)
    {
        generalType currentType;

		if (logtype == typeOfLog.touchpadMoveObject || logtype == typeOfLog.touchpadRotateObject || logtype == typeOfLog.touchpadRotateStage || logtype == typeOfLog.touchpadScaleStage)
        {
			currentType = generalType.touchpad; 
        } else
        {
            currentType = generalType.triggerInteraction;
        }

		string text = UserID + "," + currentType + "," + (Time.time - countTimeFrom) + "," + logtype + "," + selection.currentSettingSelectionMode + "," + stageMovement.currentStageMovement + "," + stageController.currentRotationScalingTechnique + "," + sessionNumber + "\n";
        
		File.AppendAllText(filePath, text);
    }
}
