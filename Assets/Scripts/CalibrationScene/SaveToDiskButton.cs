using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Progress;
using UnityEngine;

public class SaveToDiskButton : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData)
    {
        switch (TargetsManager.Instance.calibrationMode) {
			case TargetsManager.CalibrationMode.COLUMN:
				DiskAnchorsManager.Instance.SaveAllColumnAnchorsToDisk();
				break;
			case TargetsManager.CalibrationMode.PANEL:
				DiskAnchorsManager.Instance.SaveAllPanelAnchorsToDisk();
				break;
		}
    }
}
