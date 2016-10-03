using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

public class Logger : Singleton<Logger> {

	public enum typeOfLog { triggerOnObject, triggerNoTarget, touchpadScaleStage, touchpadRotateStage, touchpadMoveObject, touchpadRotateObject, nonUniformScaleHandle, RotationHandle, FrustumHandle, stage, uiElement, gestureNavigation };
    public enum generalType { navigation, triggerInteraction };

    // private string filePathLogger = @"C:\Users\user\Documents\MasterThesis Xaver - new Repo\";
    private string filePathLogger = @"L:\Master Thesis\New Git\";
    private string filePathSave = @"L:\Master Thesis\New Git\";
    private string filePathImport = @"L:\Master Thesis\New Git\CastleSave1_session1_0.xml";
    public string UserID;
    private FileStream fs;

    private float countTimeFrom;

    public StageFreeMovement stageMovement;
    public StageController stageController;
    public Selection selection;
	public int sessionNumber;
    public int saveNumber = 0;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveCurrentState();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ObjectCreator.Instance.ImportFromXML(filePathImport);
        }
    }

    public void RestartTimer()
    {
        countTimeFrom = Time.time;
    }

    public void CreateTextFile()
    {
        filePathLogger += "LogUser" + UserID + "_session" + sessionNumber + ".csv";

		RestartTimer ();
		fs = File.Create(filePathLogger);  
		fs.Dispose ();
	 	
		String header = "UserID, General Type of Interaction, Time, Specific Type of Interaction, Settings Selection Mode, Stage movement, stage rotation and scaling, session number \n";
		File.AppendAllText(filePathLogger, header);
    }

    public void AddLine(Logger.typeOfLog logtype)
    {
        generalType currentType;

		if (logtype == typeOfLog.touchpadMoveObject || logtype == typeOfLog.touchpadRotateObject || logtype == typeOfLog.touchpadRotateStage || logtype == typeOfLog.touchpadScaleStage  || logtype == typeOfLog.gestureNavigation)
        {
			currentType = generalType.navigation; 
        } else
        {
            currentType = generalType.triggerInteraction;
        }

		string text = UserID + "," + currentType + "," + (Time.time - countTimeFrom) + "," + logtype + "," + selection.currentSettingSelectionMode + "," + stageMovement.currentStageMovement + "," + stageController.currentRotationScalingTechnique + "," + sessionNumber + "\n";
        
		File.AppendAllText(filePathLogger, text);
    }


    public void SaveCurrentState()
    {
        Debug.Log("Save castle");
        String fullPathForSave = filePathSave + "CastleSave" + UserID + "_session" + sessionNumber + "_" + saveNumber + ".xml";
        saveNumber++;

        fs = File.Create(fullPathForSave);
        fs.Dispose();

        // create the string 

        //String xmlRepresentation = "<?xml version="1.0" encoding="UTF - 8"?>\n";

        String xmlRepresentation = "";
        xmlRepresentation += "<castle>\n";
        xmlRepresentation += "<userID>" + UserID + "</userID>\n";
        xmlRepresentation += "<session>" + sessionNumber + "</session>\n";
        xmlRepresentation += "<creation>\n";

        foreach (Transform child in ObjectsManager.Instance.transform)
        {       
            if (child.CompareTag("ModelingObject"))
            {
                ModelingObject currentModelingobject = child.GetComponent<ModelingObject>();
                xmlRepresentation += CreateObjectXML(currentModelingobject);

            } else
            {
                // group
                foreach (Transform groupChild in child)
                {
                    ModelingObject currentModelingobject = groupChild.GetComponent<ModelingObject>();
                    xmlRepresentation += CreateObjectXML(currentModelingobject);
                }
            }
        }
        xmlRepresentation += "</creation>\n";

        xmlRepresentation += "</castle>\n";

        File.AppendAllText(fullPathForSave, xmlRepresentation);
    }


    public String CreateObjectXML(ModelingObject currentModelingobject)
    {
        String xmlRepresentation = "";

        // single object
        xmlRepresentation += "<object>\n";

        xmlRepresentation += "<objectId>" + currentModelingobject.ObjectID + "</objectId>\n";
        xmlRepresentation += "<objectType>" + currentModelingobject.typeOfObject + "</objectType>\n";
        xmlRepresentation += "<position>";

        // we need to change it to local position, or safe stage size
        xmlRepresentation += "<x>" + currentModelingobject.transform.position.x + "</x> \n";
        xmlRepresentation += "<y>" + currentModelingobject.transform.position.y + "</y> \n";
        xmlRepresentation += "<z>" + currentModelingobject.transform.position.z + "</z> \n";
        xmlRepresentation += "</position>\n";
        xmlRepresentation += "<color>";
        xmlRepresentation += "<r>" + currentModelingobject.currentColor.r + "</r> \n";
        xmlRepresentation += "<g>" + currentModelingobject.currentColor.g + "</g> \n";
        xmlRepresentation += "<b>" + currentModelingobject.currentColor.b + "</b> \n";
        xmlRepresentation += "<a>" + currentModelingobject.currentColor.a + "</a> \n";
        xmlRepresentation += "</color>";

        xmlRepresentation += "<topfacecenter>\n";
        xmlRepresentation += "<x>" + currentModelingobject.transform.position.x + "</x> \n";
        xmlRepresentation += "<y>" + currentModelingobject.transform.position.y + "</y> \n";
        xmlRepresentation += "<z>" + currentModelingobject.transform.position.z + "</z> \n";
        xmlRepresentation += "</topfacecenter>\n";

        xmlRepresentation += "<bottomfacecenter>\n";
        xmlRepresentation += "<x>" + currentModelingobject.transform.position.x + "</x> \n";
        xmlRepresentation += "<y>" + currentModelingobject.transform.position.y + "</y> \n";
        xmlRepresentation += "<z>" + currentModelingobject.transform.position.z + "</z> \n";
        xmlRepresentation += "</bottomfacecenter>\n";

        xmlRepresentation += "<faces>\n";
        xmlRepresentation += "<topface>\n";
        xmlRepresentation += "<vertices>\n";
        for (int i = 0; i < currentModelingobject.topFace.vertexBundles.Length; i++)
        {
            xmlRepresentation += "<vertex> \n";
            xmlRepresentation += "<x>" + currentModelingobject.topFace.vertexBundles[i].coordinates.x + "</x> \n";
            xmlRepresentation += "<y>" + currentModelingobject.topFace.vertexBundles[i].coordinates.y + "</y> \n";
            xmlRepresentation += "<z>" + currentModelingobject.topFace.vertexBundles[i].coordinates.z + "</z> \n";
            xmlRepresentation += "</vertex> \n";
        }
        xmlRepresentation += "</vertices>\n";
        xmlRepresentation += "</topface> \n";

        xmlRepresentation += "<bottomface>\n";
        xmlRepresentation += "<vertices>\n";
        for (int i = 0; i < currentModelingobject.bottomFace.vertexBundles.Length; i++)
        {
            xmlRepresentation += "<vertex> \n";
            xmlRepresentation += "<x>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.x + "</x> \n";
            xmlRepresentation += "<y>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.y + "</y> \n";
            xmlRepresentation += "<z>" + currentModelingobject.bottomFace.vertexBundles[i].coordinates.z + "</z> \n";
            xmlRepresentation += "</vertex> \n";
        }
        xmlRepresentation += "</vertices>\n";
        xmlRepresentation += "</bottomface>\n";
        xmlRepresentation += "</faces>\n";
        xmlRepresentation += "</object>\n";

        return xmlRepresentation;
    }
}
