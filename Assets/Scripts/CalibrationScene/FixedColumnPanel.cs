using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

// GameObject wired to serve as panel in augmented column. Instantiated
// when tapped on Column prefab in order to lock column in position.
public class FixedColumnPanel : MonoBehaviour, IInputClickHandler {
	
	// Stores the corresponding Vuforia Image Target
	private GameObject imageTarget;

	public void RegisterImageTarget(GameObject imageTarget) {
		this.imageTarget = imageTarget;

		if (imageTarget.activeSelf) {
			imageTarget.SetActive(false);		
		}
	}

	// Removes anchor and re-enable the corresponding Vuforia Image Target
	// in order to allow further adjusting.
    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		// WorldAnchorManager.Instance.RemoveAnchor(imageTarget.name);
		// imageTarget.SetActive(true);
		Destroy(transform.parent.gameObject);
    }

	void OnDestroy() {
		WorldAnchorManager anchorManager = WorldAnchorManager.Instance;
		
		if (anchorManager != null && imageTarget != null) {
			anchorManager.RemoveAnchor(imageTarget.name);
		}

		if (imageTarget != null) {
			imageTarget.SetActive(true);
		}
	}
}
