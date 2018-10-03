using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class Tweet : MonoBehaviour, IManipulationHandler {

	LineRenderer leash;

	// Anchor can be to a fixed point  or attached to a moving object.
	bool isStationaryAnchor = false;

	// if isStationaryAnchor, then fixed point will be stored here.
	Vector3 globalAnchorPoint;
	// if !isStationaryAnchor, then moving object's transform will be stored here.
	Transform anchorTransform;

	private TextMesh messageTextMesh;
	private TextMesh usernameTextMesh;
	private Toolbar toolbar;
	private float borderSize = 0.02f;
	private readonly string usernameObjectName = "Username";
	private readonly string messageObjectName = "Message";
	private Vector3 manipulationOriginalPosition = Vector3.zero;
	public bool isManipulationEnabled {get; set;}

	void Start() {
		isManipulationEnabled = true;
	}

	void Update() {
		if (leash != null) {
			Vector3 anchorPosition = isStationaryAnchor 
				? transform.InverseTransformPoint(this.globalAnchorPoint)
				: transform.InverseTransformPoint(anchorTransform.position);

			leash.SetPosition(0, anchorPosition);

			// Leashed to the bottom right of the anchor transform.
			leash.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));	
		}			
	}

	void Awake () {
		toolbar = GetComponentInChildren<Toolbar>();

		foreach (Transform child in transform) {
			if (child.gameObject.name == usernameObjectName) {
				usernameTextMesh = child.gameObject.GetComponent<TextMesh>();
				continue;
			}

			if (child.gameObject.name == messageObjectName) {
				messageTextMesh = child.gameObject.GetComponent<TextMesh>();
				continue;
			}
		}

		if (messageTextMesh == null) {
			Debug.LogError("Cannot find message text mesh!");
		}

		if (usernameTextMesh == null) {
			Debug.LogError("Cannot find username text mesh!");
		}
	}

	public void SetUsername(string username) {
		usernameTextMesh.text = username;
	}

	public string GetUsername() {
		return usernameTextMesh.text;
	}

	public void SetMessage(string message) {
		WrapAndDisplayMessage(message, messageTextMesh);
	}

	// Inserts mesage character by character into the text mesh. Applies word wrap check
	// after inserting each character. BoxCollider seems like the only reliable way to determine
	// the width of the text displayed. Hence, we destroy and re-insert a new BoxCollider to
	// determine the current width of the text displayed.
	private void WrapAndDisplayMessage(string message, TextMesh textMesh) {
		string builder = "";
		textMesh.text = "";
		string[] parts = message.Split(' ');
		BoxCollider oldCollider = textMesh.gameObject.GetComponent<BoxCollider>();
		for (int i = 0; i < parts.Length; i++) {
			builder = textMesh.text;
			textMesh.text += parts[i] + " ";
			BoxCollider newCollider = textMesh.gameObject.AddComponent<BoxCollider>();
			Vector3 newCenter = transform.InverseTransformPoint(textMesh.transform.TransformPoint(newCollider.center));
			if (newCenter.x > 0) {
				builder = builder.TrimEnd() + System.Environment.NewLine + parts[i] + " ";
				textMesh.text = builder;
			}			
			Destroy(oldCollider);
			
		}
	}

	public void OnDestroy() {
		if (leash != null && leash.gameObject != null) {
			Destroy(leash.gameObject);
		}

		if (toolbar != null) {
			Transform button = transform.Find("Toolbar/UserHistoryButton");
			if (button != null) {
				button.gameObject.GetComponent<UserHistoryButton>().DestroyUserHistory();
			}
		}
	}

	public void AddAnchor(Transform anchorTransform) {
		GameObject lineObj = new GameObject();
		lineObj.transform.SetParent(transform, false);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
        line.startWidth = 0.005f;
		this.anchorTransform = anchorTransform;
		this.isStationaryAnchor = false;
        this.leash = line;
	}

	public void AddAnchor(Vector3 anchorPoint) {
		GameObject lineObj = new GameObject();
		lineObj.transform.SetParent(transform, false);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
        line.startWidth = 0.005f;
		this.globalAnchorPoint = anchorPoint;
		this.isStationaryAnchor = true;
        this.leash = line;
	}


    void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {	
        if (isManipulationEnabled)
        {
			if (eventData.used) {
				return;
			}
			eventData.Use();

            InputManager.Instance.PushModalInputHandler(gameObject);

            manipulationOriginalPosition = transform.position;
        }
    }

    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (isManipulationEnabled)
        {
			if (eventData.used) {
				return;
			}
			eventData.Use();

            transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
        }
    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }
}
