using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitterMainMenuButton : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
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
