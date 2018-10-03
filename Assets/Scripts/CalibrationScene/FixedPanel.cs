using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

// GameObject wired to serve as panel in augmented panel. Instantiated
// when tapped on Panel prefab in order to lock panel in position.
public class FixedPanel : MonoBehaviour, IInputClickHandler {

	// Stores the corresponding Vuforia Image Target
	private GameObject imageTarget;

	public void RegisterImageTarget(GameObject imageTarget) {
		this.imageTarget = imageTarget;

		if (imageTarget.activeSelf) {
			imageTarget.SetActive(false);		
		}
	}

	
    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		// WorldAnchorManager.Instance.RemoveAnchor(imageTarget.name);
		// imageTarget.SetActive(true);
		Destroy(gameObject);
    }

	// Removes anchor and re-enable the corresponding Vuforia Image Target
	// in order to allow further adjusting.
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
