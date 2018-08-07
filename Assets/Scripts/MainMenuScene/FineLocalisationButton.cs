using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FineLocalisationButton : MonoBehaviour, IInputClickHandler {
	public void OnInputClicked(InputClickedEventData eventData)
    {
        StartCoroutine(LoadFineLocalisationSceneAsync());
    }

	IEnumerator LoadFineLocalisationSceneAsync()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.AnimatedOrbs,
                            ProgressStyleEnum.None,
                            ProgressMessageStyleEnum.Visible,
                            "Loading Fine Localisation Scene.");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("FineLocalisationScene");

        asyncLoad.completed += (AsyncOperation op) => {
			Debug.Log("Closing progressIndicator instance.");
			ProgressIndicator.Instance.Close();
		};

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}	
