using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using UnityEngine;

// Recurring toolbar used to hold Holographic Buttons.
public class Toolbar : MonoBehaviour {

	private float buttonHeight;
	private float buttonWidth;

	public enum ButtonLayout {
		VERTICAL,
		HORIZONTAL
	}

	[SerializeField]
	private ButtonLayout layout = ButtonLayout.VERTICAL;

	private BoxCollider collider;

	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider>();

		Transform button = transform.GetChild(0);
		buttonHeight = button.GetComponent<BoxCollider>().size.y * button.localScale.y;
		buttonWidth = button.GetComponent<BoxCollider>().size.x * button.localScale.x;

		PositionActiveButtons();
	}

	public void PositionActiveButtons() {
		List<Transform> activeButtons = new List<Transform>();

		foreach (Transform child in transform) {
			if (child.gameObject.activeSelf) {
				activeButtons.Add(child);
			}
		}

		switch (layout) {
			case ButtonLayout.VERTICAL:
				PositionActiveButtonsVertically(activeButtons);

				if (collider != null) {
					collider.size = new Vector3(collider.size.x, activeButtons.Count * buttonHeight, collider.size.z);
				}				
				break;
			case ButtonLayout.HORIZONTAL:
				PositionActiveButtonsHorizontally(activeButtons);

				if (collider != null) {
					collider.size = new Vector3(activeButtons.Count * buttonWidth, collider.size.y, collider.size.z);
				}
				break;
		}
	}
	
	private void PositionActiveButtonsVertically(List<Transform> activeButtons) {
		float startPos = buttonHeight * (activeButtons.Count) / 2 - buttonHeight / 2;

		for (int i = 0; i < activeButtons.Count; i++) {
			activeButtons[i].localPosition = new Vector3(activeButtons[i].localPosition.x
				, startPos - buttonHeight * i
				, activeButtons[i].localPosition.z);
		}
	}

	private void PositionActiveButtonsHorizontally(List<Transform> activeButtons) {
		float startPos = -buttonWidth * (activeButtons.Count) / 2 + buttonWidth / 2;

		for (int i = 0; i < activeButtons.Count; i++) {
			activeButtons[i].localPosition = new Vector3(startPos + buttonWidth * i
				, activeButtons[i].localPosition.y
				, activeButtons[i].localPosition.z);
		}
	}
	
	public void DisableAllButtons() {
		foreach (Transform child in transform) {
			CompoundButton button = child.gameObject.GetComponent<CompoundButton>();
			if (button != null) {
				button.MainCollider.enabled = false;
			}
		}
	}

	public void EnableAllButtons() {
		foreach (Transform child in transform) {
			CompoundButton button = child.gameObject.GetComponent<CompoundButton>();
			if (button != null) {
				button.MainCollider.enabled = true;
			}
		}
	}
}
