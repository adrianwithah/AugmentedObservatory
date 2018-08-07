using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MovePanelAction : MonoBehaviour, IManipulationHandler {

	private Vector3 manipulationOriginalPosition = Vector3.zero;

	public bool isManipulationEnabled {get; set;}

    void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {	
		Debug.Log("Manipulation start detected!");
        if (isManipulationEnabled)
        {
            InputManager.Instance.PushModalInputHandler(gameObject);

            manipulationOriginalPosition = transform.position;
        }
    }

    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (isManipulationEnabled)
        {
            /* TODO: DEVELOPER CODING EXERCISE 4.a */

            // 4.a: Make this transform's position be the manipulationOriginalPosition + eventData.CumulativeDelta
            transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
        }
    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();

        ModifiedBoundingBoxRig rig;
        if ((rig = this.gameObject.GetComponent<ModifiedBoundingBoxRig>()) != null) {

        }
    }

    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }


    // Use this for initialization
    void Start () {
		isManipulationEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
