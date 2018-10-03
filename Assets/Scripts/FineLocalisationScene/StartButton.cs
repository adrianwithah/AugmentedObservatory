using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

// Button to start the test.
public class StartButton : MonoBehaviour, IInputClickHandler {

	public void OnInputClicked(InputClickedEventData eventData)
    {
        if (eventData.used) {
			return;
		}

		eventData.Use();
		
		TestManager.Instance.StartTest();
    }
}
