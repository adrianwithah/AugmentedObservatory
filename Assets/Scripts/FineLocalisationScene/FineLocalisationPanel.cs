using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class FineLocalisationPanel : MonoBehaviour, IInputClickHandler {
	public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		RaycastHit hit = GazeManager.Instance.HitInfo;
		CustomAudioManager.Instance.PlayInputClicked();
		TestManager.Instance.SubmitCoordinatesPressed(hit.collider.transform.InverseTransformPoint(hit.point), gameObject);
    }
}
