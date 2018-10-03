using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using HoloToolkit.Unity;

// GameObject responsible for setting up/track Vuforia Image Target integrations with
// the application.
public class TargetsManager : NonPersistSingleton<TargetsManager> {

    // Panel calibration mode was created to test against the column calibration mode.
    // Panel mode will remain the application as a slightly more accurate alternative
    // in case of need.
    public enum CalibrationMode {
        COLUMN,
        PANEL
    }
    public CalibrationMode calibrationMode;

    // Arrays to store the image targets, in order of the image target indexes.
    // E.g. columnImageTargets[0] contains image target column0, etc.
    private const int maxNumOfImageTargets = 64;
    private GameObject[] imageTargets = new GameObject[maxNumOfImageTargets];
    private const int maxNumOfColumnImageTargets = 16;
    private GameObject[] columnImageTargets = new GameObject[maxNumOfColumnImageTargets];

    // Both calibration modes will share the same set of QR code
    // markers. The only difference is that when in Column calibration mode, the data
    // set only contains image targets corresponding to markers 32 to 47
    // (second row from bottom of Observatory).
    public readonly static int secondRowStartIndex = 32;
    public readonly static int secondRowEndIndex = 47;

    // The naming convention used to name Vuforia Image Targets.
    public readonly static string columnImageTargetPrefix = "column";
    public readonly static string panelImageTargetPrefix = "panel";

    // Name of the data set used.
    private readonly string columnImageTargetsDataSet = "ColumnImageTargets";
    private readonly string panelImageTargetsDataSet = "64ImageTargets";

    [SerializeField]
    private GameObject vuforiaColumn;
    [SerializeField]
    private GameObject vuforiaPanel;

    void Start() {
        switch (calibrationMode) {
            case CalibrationMode.COLUMN:
                VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadColumnImageTargetsDataSet);
                break;
            case CalibrationMode.PANEL:
                VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadPanelImageTargetsDataSet);
                break;
        }
    }

    // Loads the Image Targets dataset and attaches the corresponding object to
    // be augmented on target recognition.
    void LoadColumnImageTargetsDataSet() {
 
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
         
        DataSet dataSet = objectTracker.CreateDataSet();
        
        if (dataSet.Load(columnImageTargetsDataSet)) {

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

                    if (vuforiaColumn != null) {
                        // instantiate augmentation object and parent to trackable
                        GameObject.Instantiate(vuforiaColumn, tb.gameObject.transform);
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

    void LoadPanelImageTargetsDataSet()
    {
 
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
         
        DataSet dataSet = objectTracker.CreateDataSet();
        
        if (dataSet.Load(panelImageTargetsDataSet)) {

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

                    if (vuforiaPanel != null) {
                        // instantiate augmentation object and parent to trackable
                        GameObject.Instantiate(vuforiaPanel, tb.gameObject.transform);
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

    // Add Panel Image Target to tracking array. Checks for valid and unique index.
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

    // Add Column Image Target to tracking array. Checks for valid and unique index.
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

    // Note: Panel numbering for this application starts from top left and increases
    // from left to right.

    // From the Panel Image Target name.
    public static int GetPanelNumberFromPanelName(string panelName) {
        return System.Int32.Parse(panelName.Substring(5));
    }

    // Method to retrieve corresponding column number from the Panel Image Target name.
    public static int GetColumnNumberFromPanelName(string panelName) {
        int panelNum = GetPanelNumberFromPanelName(panelName);

        if (panelNum < TargetsManager.secondRowStartIndex || panelNum > secondRowEndIndex) {
            Debug.LogFormat("Panel number: {0} is not being used to augment columns!", panelNum);
            return -1;
        }

        return panelNum - secondRowStartIndex;
    }

    // From the Column Image Target name.
    public static int GetColumnNumberFromColumnName(string columnName) {
        return System.Int32.Parse(columnName.Substring(6));
    }

    // Method for unloading active data sets once user exits the scene. This is to prevent
    // presence of multiple copies of same data sets when re-entering scene.
    public void UnloadActiveDataSets() {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        System.Collections.ObjectModel.ReadOnlyCollection<DataSet> activeDataSets = objectTracker.GetActiveDataSets().ToReadOnlyCollection();

        for (int i = 0; i < activeDataSets.Count; i++) {
            DataSet currDataSet = activeDataSets[i];

            StateManager stateManager = TrackerManager.Instance.GetStateManager();
            List<TrackableBehaviour> tbs = (List<TrackableBehaviour>) TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

            for (int j = 0; j < tbs.Count; j++) {
                if (activeDataSets[i].Contains(tbs[j].Trackable)) {
                    stateManager.DestroyTrackableBehavioursForTrackable(tbs[j].Trackable, true);
                }
            }
            
            if (!objectTracker.DeactivateDataSet(currDataSet)) {
                Debug.Log("Could not deactive data set!");
            }

            if (!objectTracker.DestroyDataSet(currDataSet, false)) {
                Debug.Log("Could not destroy data set!");
            }
        }
    }

    // Method to determine and link up the corresponding image targets, after loading
    // store World Anchors and attaching to them to column objects.
    public void RegisterColumnImageTargets(Dictionary<string, GameObject> anchorIdToObject) {
		foreach (string anchorId in anchorIdToObject.Keys) {
			GameObject imageTarget 
				= GetColumnImageTarget(GetColumnNumberFromColumnName(anchorId));

			if (imageTarget == null) {
				Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped loading anchor.", anchorId);
			}

			GameObject anchoredObject;
			
			if (!anchorIdToObject.TryGetValue(anchorId, out anchoredObject)) {
				Debug.LogError("Error loading object from dictionary!");
				continue;
			}

			foreach (Transform childTransform in anchoredObject.transform) {
				Debug.LogFormat("Adding fixed column panel for child: {0}", childTransform.gameObject.name);
				childTransform.gameObject
					.EnsureComponent<FixedColumnPanel>()
					.RegisterImageTarget(imageTarget);
			}
		}
	}

    // Method to determine and link up the corresponding image targets, after loading
    // store World Anchors and attaching to them to panel objects.
    public void RegisterPanelImageTargets(Dictionary<string, GameObject> anchorIdToObject) {
		foreach (string anchorId in anchorIdToObject.Keys) {

			GameObject imageTarget = TargetsManager.Instance.GetImageTarget(
                TargetsManager.GetPanelNumberFromPanelName(anchorId));
			if (imageTarget == null) {
				Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped anchor.", anchorId);
				continue;
			}

			GameObject anchoredObject;
			
			if (!anchorIdToObject.TryGetValue(anchorId, out anchoredObject)) {
				Debug.LogError("Error loading object from dictionary!");
				continue;
			}

			anchoredObject.EnsureComponent<FixedPanel>().RegisterImageTarget(imageTarget);
		}
	}
}