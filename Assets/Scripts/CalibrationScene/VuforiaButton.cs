using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using Vuforia;
using HoloToolkit.Unity.Buttons;

// Functionality to disable/enable feeding the camera stream into Vuforia.
// This was built to temporarily enable Mixed Reality Capture/Photo-Taking on the HoloLens,
// while in the CalibrationScene.
public class VuforiaButton : MonoBehaviour, IInputClickHandler {

	private bool vuforiaOn = true;
	private CompoundButtonText buttonText;

	void Start () {
		buttonText = gameObject.GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonText>();
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		if (CameraDevice.Instance == null) {
			buttonText.Text = "Vuforia Failed";
		}

        if (vuforiaOn) {
			CameraDevice.Instance.Stop();
			CameraDevice.Instance.Deinit();
			buttonText.Text = "Vuforia Off";
		} else {
			CameraDevice.Instance.Init();
			CameraDevice.Instance.Start();
			buttonText.Text = "Vuforia On";
		}

		vuforiaOn = !vuforiaOn;
    }
}
