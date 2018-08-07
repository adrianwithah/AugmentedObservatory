using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Dialog;
using UnityEngine;
using UnityEngine.XR.WSA;

public class LoadFromDiskButton : MonoBehaviour, IInputClickHandler {
	[SerializeField]
	private Dialog dialogPrefab;

    public void OnInputClicked(InputClickedEventData eventData)
    {
		//Prevent loading from disk if there already are existing WorldAnchors. This is to prevent misuse.
		if (FindObjectOfType<WorldAnchor>() != null) {
			
			Dialog.Open(dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Please remove all remaining World Anchors from scene.");

			return;
		}

        switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:
				DiskAnchorsManager.Instance.LoadAllColumnAnchorsFromDisk();
				break;
			case TargetsManager.CalibrationMode.PANEL:
				DiskAnchorsManager.Instance.LoadAllPanelAnchorsFromDisk();
				break;
		}
    }
}
