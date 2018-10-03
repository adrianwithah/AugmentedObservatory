using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;

public class LoadColumnButton : MonoBehaviour, IInputClickHandler {

	[SerializeField]
	private GameObject fineLocalColumn;

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = false;
		
		AnchorsManager.Instance.LoadAllColumnAnchorsFromStore(fineLocalColumn);

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }
}
