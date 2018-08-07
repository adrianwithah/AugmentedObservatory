using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;

public class AdjustPanelAction : MonoBehaviour, IInputHandler, IFocusable, IManipulationHandler {

	bool inputDown = false;

	private Vector3 manipulationOriginalPosition = Vector3.zero;

	// [Tooltip("Rotation max speed controls amount of rotation.")]
    // [SerializeField]
    // private float RotationSensitivity = 10.0f;
    
    // void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
    // {   
    //     Debug.Log("Navigation Start detected!");
    //     InputManager.Instance.PushModalInputHandler(gameObject);
    // }

    // void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
    // {
    //     InputManager.Instance.PopModalInputHandler();
    // }

    // void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
    // {
    //     InputManager.Instance.PopModalInputHandler();
    // }

    // public void OnNavigationUpdated(NavigationEventData eventData)
    // {
    //     if (isNavigationEnabled) {
    //         //todo: implement 3d rotation smoothly.
    //         float rotationFactor = eventData.NormalizedOffset.x * RotationSensitivity;
	// 	    transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
    //     }
    // }

    void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {	
		InputManager.Instance.PushModalInputHandler(gameObject);

		manipulationOriginalPosition = transform.position;
    }

    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
		transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter() {
        
    }

    public void OnFocusExit() {
        inputDown = false;

		this.gameObject.GetComponent<BoundingBoxRig>().Deactivate();
    }

    public void OnInputDown(InputEventData eventData) {
		Debug.Log("Input down");
		inputDown = true;
    }

    public void OnInputUp(InputEventData eventData) {
		Debug.Log("Input up");
        if (inputDown) {
			InputClicked();
		}

		inputDown = false;
    }

	private void InputClicked() {
		this.gameObject.GetComponent<BoundingBoxRig>().Activate();
	}
}
