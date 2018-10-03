using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;

public class LoadPanelButton : MonoBehaviour, IInputClickHandler {

	[SerializeField]
	private GameObject fineLocalPanel;

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = false;	

		AnchorsManager.Instance.LoadAllPanelAnchorsFromStore(fineLocalPanel);

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }
}
