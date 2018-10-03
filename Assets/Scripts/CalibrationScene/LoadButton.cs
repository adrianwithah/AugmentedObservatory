using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;

// Responsible for loading saved WorldAnchors from the App's WorldAnchorStore.
public class LoadButton : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		// Disables (and later re-enable) button in order to prevent simultaneous requests.
		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = false;

		switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:

				TargetsManager.Instance.RegisterColumnImageTargets(
					AnchorsManager.Instance
					.LoadAllColumnAnchorsFromStore(PrefabsManager.Instance.fixedColumn)
				);

				break;
			case TargetsManager.CalibrationMode.PANEL:
				
				TargetsManager.Instance.RegisterPanelImageTargets(
					AnchorsManager.Instance
					.LoadAllPanelAnchorsFromStore(PrefabsManager.Instance.fixedPanel)
				);

				break;
		}

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }
}
