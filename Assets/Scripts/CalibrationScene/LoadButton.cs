using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;

public class LoadButton : MonoBehaviour, IInputClickHandler {

	public GameObject columnObjectToAnchor;
	public GameObject panelObjectToAnchor;
	private TargetsManager targetsManager;

	// Use this for initialization
    void Start () {
		targetsManager = GameObject.Find("TargetsManager").GetComponent<TargetsManager>();

		if (targetsManager == null) {
			Debug.Log("Cannot find targetsManager!");
		}
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		Debug.Log("CLicked!");
		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = false;

		switch (targetsManager.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:
				LoadAllColumnAnchors();
				break;
			case TargetsManager.CalibrationMode.PANEL:
				LoadAllPanelAnchors();
				break;
		}

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }

	void LoadAllColumnAnchors() {
		Debug.Log("Trying to load all column anchors from the store..");
		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return;
		}

		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {
			
			if (!ids[i].StartsWith(TargetsManager.columnImageTargetPrefix)) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}

			GameObject imageTarget 
				= targetsManager.GetColumnImageTarget(TargetsManager.GetColumnNumberFromColumnName(ids[i]));

			if (imageTarget == null) {
				Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped loading anchor.", ids[i]);
				continue;
			}

			GameObject clone = GameObject.Instantiate(columnObjectToAnchor);
			foreach (Transform childTransform in clone.transform) {
				Debug.LogFormat("Adding fixed column panel for child: {0}", childTransform.gameObject.name);
				childTransform.gameObject.GetComponent<FixedColumnPanel>().RegisterImageTarget(imageTarget);
			}
			clone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

			if (store.Load(ids[i], clone) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Column with id: {0}.", ids[i]);
			}
		}
	}

	void LoadAllPanelAnchors() {
		Debug.Log("Trying to load all panel anchors from the store..");
		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return;
		}

		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {

			if (!ids[i].StartsWith(TargetsManager.panelImageTargetPrefix)) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}
			
			GameObject imageTarget = targetsManager.GetImageTarget(TargetsManager.GetPanelNumberFromPanelName(ids[i]));
			if (imageTarget == null) {
				Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped loading anchor.", ids[i]);
				continue;
			}

			Debug.LogFormat("id: {0}", ids[i]);
			GameObject clone = GameObject.Instantiate(panelObjectToAnchor
				, panelObjectToAnchor.transform.position
				, panelObjectToAnchor.transform.rotation);
			clone.transform.localScale = new Vector3 (1.1f, 0.6f, 0.2f);

			clone.GetComponent<FixedPanel>().RegisterImageTarget(imageTarget);

			if (store.Load(ids[i], clone) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Panel with id: {0}", ids[i]);
			}
		}
	}
}
