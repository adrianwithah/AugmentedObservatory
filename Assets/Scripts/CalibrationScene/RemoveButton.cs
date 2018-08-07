using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class RemoveButton : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData) {
        WorldAnchorManager.Instance.RemoveAllObjectsWithAnchors();
    }
}
