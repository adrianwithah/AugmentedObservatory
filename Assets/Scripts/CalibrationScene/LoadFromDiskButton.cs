using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Dialog;
using UnityEngine;
using UnityEngine.XR.WSA;

// Responsible for loading WorldAnchors from .dat file containing
// serialized WorldAnchors on the HoloLens.
public class LoadFromDiskButton : MonoBehaviour, IInputClickHandler {

	private GameObject toolbar;

	void Start() {
		toolbar = transform.parent.gameObject;
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		// Prevent loading from disk if there already are existing WorldAnchors. 
		// This is to ensure that all existing WorldAnchors are non-overlapping.
		if (FindObjectOfType<WorldAnchor>() != null) {
			
			Dialog.Open(PrefabsManager.Instance.dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Please remove all anchored objects from scene before loading.");

			return;
		}

		toolbar.GetComponent<Toolbar>().DisableAllButtons();
		toolbar.GetComponent<Tagalong>().enabled = false;

		AnchorsManager.Instance.DiskLoadCompletedAction += OnLoadCompleted;

        switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:
				AnchorsManager.Instance.LoadAllColumnAnchorsFromDisk(PrefabsManager.Instance.fixedColumn);
				break;
			case TargetsManager.CalibrationMode.PANEL:
				AnchorsManager.Instance.LoadAllPanelAnchorsFromDisk(PrefabsManager.Instance.fixedPanel);
				break;
		}
    }

	// Toolbar is disabled during save to prevent problems arising due to
	// concurrency issues with async disk operations. Re-enabled after save completed.
	private void OnLoadCompleted(Dictionary<string, GameObject> anchorIdToObject) {

		switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:
				TargetsManager.Instance.RegisterColumnImageTargets(anchorIdToObject);
				break;
			case TargetsManager.CalibrationMode.PANEL:
				TargetsManager.Instance.RegisterPanelImageTargets(anchorIdToObject);
				break;
		}

		toolbar.GetComponent<Toolbar>().EnableAllButtons();
		toolbar.GetComponent<Tagalong>().enabled = true;
		AnchorsManager.Instance.DiskLoadCompletedAction -= OnLoadCompleted;
	}
}
