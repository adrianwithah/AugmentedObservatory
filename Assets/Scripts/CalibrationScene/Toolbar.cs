using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using UnityEngine;

public class Toolbar : MonoBehaviour {

	// private readonly string saveToDiskButtonName = "SaveToDiskButton";
	// private readonly string loadButtonName = "LoadButton";
	// private readonly string removeButtonName = "RemoveButton";
	// private readonly string vuforiaButtonName = "VuforiaButton";
	// private readonly string loadFromDiskButtonName = "LoadFromDiskButton";

	// private GameObject saveToDiskButton;
	// private GameObject loadButton;
	// private GameObject removeButton;
	// private GameObject vuforiaButton;
	// private GameObject loadFromDiskButton;

	// public GameObject GetSaveToDiskButton() {
	// 	return saveToDiskButton;
	// }

	// public GameObject GetLoadButton() {
	// 	return loadButton;
	// }

	// public GameObject GetRemoveButton() {
	// 	return removeButton;
	// }

	// public GameObject GetVuforiaButton() {
	// 	return vuforiaButton;
	// }

	// public GameObject GetLoadFromDiskButton() {
	// 	return loadFromDiskButton;
	// }

	private float buttonHeight = 0.12f;

	// Use this for initialization
	void Start () {
		PositionButtons();
	}
	
	private void PositionButtons() {
		float startPos = buttonHeight * (transform.childCount) / 2;

		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).localPosition = new Vector3(transform.GetChild(i).position.x
				, startPos - buttonHeight * i
				, transform.GetChild(i).position.z);
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

	public void PauseTagalong() {
		Tagalong tagalong = GetComponent<Tagalong>();

		if (tagalong == null) {
			return;
		}

		tagalong.enabled = false;
	}

	public void ResumeTagalong() {
		Tagalong tagalong = GetComponent<Tagalong>();

		if (tagalong == null) {
			Debug.LogError("Tagalong could not be found on toolbar!");
			return;
		}

		tagalong.enabled = false;
	}
}
