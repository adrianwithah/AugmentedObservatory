using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class RemoveStatsPanelButton : MonoBehaviour, IInputClickHandler {

	public void OnInputClicked(InputClickedEventData eventData)
    {
        Destroy(transform.parent.parent.gameObject);
    }
}
