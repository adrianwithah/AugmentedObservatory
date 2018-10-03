using System.Collections;
using System.Collections.Generic;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

// Responsible for setting up the general environment before entering any specific scene.
// Makes sure cursor persists across scenes. Also, disable Vuforia until user enters
// CalibrationScene.
public class Initialiser : MonoBehaviour {

	[SerializeField]
	private GameObject cursor;

	// Use this for initialization
	void Start () {
		if (VuforiaBehaviour.Instance != null) {
			VuforiaBehaviour.Instance.enabled = false;
		}
		
		InitialiseCursor();
		StartCoroutine(LoadMainMenuScene());
	}

	IEnumerator LoadMainMenuScene() {
		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.AnimatedOrbs,
                            ProgressStyleEnum.None,
                            ProgressMessageStyleEnum.Visible,
                            "Loading Main Menu scene.");

		AsyncOperation asyncOp = SceneManager.LoadSceneAsync("MainMenuScene");

		asyncOp.completed += (AsyncOperation op) => {
			Debug.Log("Closing progressIndicator instance.");
			ProgressIndicator.Instance.Close();
		};

		while (!asyncOp.isDone) {
			yield return null;
		}
	}

	public void InitialiseCursor() {
		DontDestroyOnLoad(cursor);
	}
}
