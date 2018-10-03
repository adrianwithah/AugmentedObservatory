using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class CalibrationMainMenuButton : MonoBehaviour, IInputClickHandler {
	void Start() {
		VuforiaBehaviour.Instance.enabled = true;
	}

    public void OnInputClicked(InputClickedEventData eventData) {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		StartCoroutine(GoToMainMenu());
    }

	IEnumerator GoToMainMenu() {
		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.AnimatedOrbs,
                            ProgressStyleEnum.None,
                            ProgressMessageStyleEnum.Visible,
                            "Going back to Main Menu scene.");

		TargetsManager.Instance.UnloadActiveDataSets();

		VuforiaBehaviour.Instance.enabled = false;

		AsyncOperation asyncOp = SceneManager.LoadSceneAsync("MainMenuScene");

		asyncOp.completed += (AsyncOperation op) => {
			Debug.Log("Closing progressIndicator instance.");
			ProgressIndicator.Instance.Close();
		};

		while (!asyncOp.isDone) {
			yield return null;
		}
	}
}
