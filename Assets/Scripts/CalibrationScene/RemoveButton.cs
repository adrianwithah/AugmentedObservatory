using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA;

public class RemoveButton : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData) {
        if (eventData.used) {
			return;
		}
		eventData.Use();
        
        // WorldAnchorManager.Instance.RemoveAllObjectsWithAnchors();
        DeleteAnchoredObjects();
    }

    private void DeleteAnchoredObjects() {
        var anchors = FindObjectsOfType<WorldAnchor>();

        if (anchors == null) {
             return;
        }

        for (var i = 0; i < anchors.Length; i++) {
            Debug.Log("Destroying gameobject for anchor: " + anchors[i].name);
            Destroy(anchors[i].gameObject);
        }
    }
}
