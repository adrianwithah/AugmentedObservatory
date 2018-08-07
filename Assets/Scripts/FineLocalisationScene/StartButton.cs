using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class StartButton : MonoBehaviour, IInputClickHandler {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnInputClicked(InputClickedEventData eventData)
    {
        if (eventData.used) {
			return;
		}

		eventData.Use();
		
		TestManager.Instance.StartTest();
    }
}
