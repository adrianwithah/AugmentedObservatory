using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class FineLocalisationPanel : MonoBehaviour, IInputClickHandler {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnInputClicked(InputClickedEventData eventData)
    {
		RaycastHit hit = GazeManager.Instance.HitInfo;
		TestManager.Instance.SubmitCoordinatesPressed(hit.collider.transform.InverseTransformPoint(hit.point), gameObject);
    }
}
