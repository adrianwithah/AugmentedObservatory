using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.XR.WSA;

// Responsible for serializing WorldAnchors and saving to
// a .dat file on the HoloLens, located at AnchorsManager.storageFolder.
public class SaveToDiskButton : MonoBehaviour, IInputClickHandler {

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

		toolbar.GetComponent<Toolbar>().DisableAllButtons();
		toolbar.GetComponent<Tagalong>().enabled = false;

		AnchorsManager.Instance.DiskSaveCompletedAction += OnSaveCompleted;

		WorldAnchor[] anchors = FindObjectsOfType<WorldAnchor>();

        switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:

				List<WorldAnchor> columnAnchors = new List<WorldAnchor>();
				foreach (WorldAnchor anchor in anchors) {
					//If its a FixedColumn, then it has 4 children.
					if (anchor.gameObject.transform.childCount != 4) {
						Debug.LogFormat("{0} does not have 4 children, cannot be FixedColumn. Skipping anchor!", anchor.name);
						continue;
					}

					if (!AnchorsManager.IsValidColumnAnchorId(anchor.gameObject.name)) {
						Debug.LogFormat("Anchor gameobject's name does not suit column image target. Skipping anchor!", anchor.name);
						continue;
					}

					columnAnchors.Add(anchor);
				}

				AnchorsManager.Instance.SaveAllColumnAnchorsToDisk(columnAnchors.ToArray());
				break;

			case TargetsManager.CalibrationMode.PANEL:

				List<WorldAnchor> panelAnchors = new List<WorldAnchor>();
				foreach (WorldAnchor anchor in anchors) {
					//Since we only anchor FixedPanel and FixedColumn, we can start by checking if it has FixedPanel.
					FixedPanel fixedPanel = anchor.gameObject.GetComponent<FixedPanel>();
					if (fixedPanel == null) {
						Debug.LogFormat("Anchor's gameobject does not have FixedPanel. Skipping anchor!", anchor.name);
						continue;
					}

					panelAnchors.Add(anchor);
				}

				AnchorsManager.Instance.SaveAllPanelAnchorsToDisk(panelAnchors.ToArray());
				break;
				
			default:
				OnSaveCompleted();
				break;
		}
    }


	// Toolbar is disabled during save to prevent problems arising due to
	// concurrency issues with async disk operations. Re-enabled after save completed.
	private void OnSaveCompleted() {
		toolbar.GetComponent<Toolbar>().EnableAllButtons();
		toolbar.GetComponent<Tagalong>().enabled = true;
		AnchorsManager.Instance.DiskSaveCompletedAction -= OnSaveCompleted;
	}
}
