using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class TargetsManager : Singleton<TargetsManager>
{
    public enum CalibrationMode {
        COLUMN,
        PANEL
    }
    // specify these in Unity Inspector
    public GameObject columnObject;
    public GameObject panelObject;  // you can use teapot or other object

    public CalibrationMode calibrationMode;
    private const int maxNumOfImageTargets = 64;
    private GameObject[] imageTargets = new GameObject[maxNumOfImageTargets];
    private const int maxNumOfColumnImageTargets = 16;
    private GameObject[] columnImageTargets = new GameObject[maxNumOfColumnImageTargets];

    public readonly static int secondRowStartIndex = 32;
    public readonly static int secondRowEndIndex = 47;
    public readonly static string columnImageTargetPrefix = "column";
    public readonly static string panelImageTargetPrefix = "panel";
 
    // Use this for initialization
    void Start()
    {
        switch (calibrationMode) {
            case CalibrationMode.COLUMN:
                VuforiaARController.Instance.RegisterVuforiaStartedCallback(Load16ImageTargetsDataSet);
                break;
            case CalibrationMode.PANEL:
                VuforiaARController.Instance.RegisterVuforiaStartedCallback(Load64ImageTargetsDataSet);
                break;
        }
    }
    void Load16ImageTargetsDataSet()
    {
 
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
         
        DataSet dataSet = objectTracker.CreateDataSet();
        
        if (dataSet.Load("16ImageTargets")) {

            objectTracker.Stop();  // stop tracker so that we can add new dataset
 
            if (!objectTracker.ActivateDataSet(dataSet)) {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet.</color>");
            }
 
            if (!objectTracker.Start()) {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs) {

                int columnNum = GetPanelNumberFromPanelName(tb.TrackableName) - secondRowStartIndex;

                if (tb.name == "New Game Object") {
                    // change generic name to include trackable name
                    tb.gameObject.name = string.Format("column{0}", columnNum);

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();

                    if (columnObject != null) {
                        // instantiate augmentation object and parent to trackable
                        GameObject.Instantiate(columnObject, tb.gameObject.transform);
                    } else {
                        Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                    }
                }

                AddColumnImageTarget(tb.gameObject, columnNum);
            }

        } else {
            Debug.LogError("<color=yellow>Failed to load dataset.</color>");
        }
    }

    void Load64ImageTargetsDataSet()
    {
 
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
         
        DataSet dataSet = objectTracker.CreateDataSet();
        
        if (dataSet.Load("64ImageTargets")) {

            objectTracker.Stop();  // stop tracker so that we can add new dataset
 
            if (!objectTracker.ActivateDataSet(dataSet)) {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet.</color>");
            }
 
            if (!objectTracker.Start()) {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs) {

                if (tb.name == "New Game Object") {
                    // change generic name to include trackable name
                    tb.gameObject.name = tb.TrackableName;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();

                    if (panelObject != null) {
                        // instantiate augmentation object and parent to trackable
                        GameObject.Instantiate(panelObject, tb.gameObject.transform);
                    } else {
                        Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                    }
                }

                AddImageTarget(tb.gameObject, GetPanelNumberFromPanelName(tb.gameObject.name));
            }

        } else {
            Debug.LogError("<color=yellow>Failed to load dataset.</color>");
        }
    }
    private void AddImageTarget(GameObject imageTarget, int index) {
        if (index < 0) {
            Debug.LogFormat("Index: {0} below 0 is invalid", index);
            return;
        }

        if (index > maxNumOfImageTargets) {
            Debug.LogFormat("Index: {0} above maxNumOfImageTargets.", index);
            return;
        }

        if (imageTargets[index] != null) {
            Debug.LogFormat("Warning! Overriding another image target at index: {0}", index);
        }
        
        imageTargets[index] = imageTarget;
    }

    private void AddColumnImageTarget(GameObject imageTarget, int index) {
        if (index < 0) {
            Debug.LogFormat("Index: {0} below 0 is invalid", index);
            return;
        }

        if (index > maxNumOfColumnImageTargets) {
            Debug.LogFormat("Index: {0} above maxNumOfColumnImageTargets.", index);
            return;
        }

        if (columnImageTargets[index] != null) {
            Debug.LogFormat("Warning! Overriding another image target at index: {0}", index);
        }
        
        columnImageTargets[index] = imageTarget;
    }

    public GameObject GetImageTarget(int index) {
        if (index < 0) {
            Debug.LogFormat("Index: {0} below 0 is invalid", index);
            return null;
        }

        if (index > maxNumOfImageTargets) {
            Debug.LogFormat("Index: {0} above maxNumOfImageTargets.", index);
            return null;
        }

        return imageTargets[index];
    }

    public GameObject GetColumnImageTarget(int index) {
        if (index < 0) {
            Debug.LogFormat("Index: {0} below 0 is invalid", index);
            return null;
        }

        if (index > maxNumOfColumnImageTargets) {
            Debug.LogFormat("Index: {0} above maxNumOfColumnImageTargets.", index);
            return null;
        }

        Debug.LogFormat("Getting column image target index: {0}", index);
        return columnImageTargets[index];
    }

    public static int GetPanelNumberFromPanelName(string panelName) {
        Debug.Log(panelName);
        return System.Int32.Parse(panelName.Substring(5));
    }

    public static int GetColumnNumberFromPanelName(string panelName) {
        int panelNum = GetPanelNumberFromPanelName(panelName);

        if (panelNum < TargetsManager.secondRowStartIndex || panelNum > secondRowEndIndex) {
            Debug.LogFormat("Panel number: {0} is not being used to augment columns!", panelNum);
            return -1;
        }

        return panelNum - secondRowStartIndex;
    }

    public static int GetColumnNumberFromColumnName(string columnName) {
        Debug.Log(columnName);
        return System.Int32.Parse(columnName.Substring(6));
    }
}