using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MinimisedQuad : MonoBehaviour, IInputClickHandler {
	LineRenderer leash;

	// Anchor can be to a fixed point  or attached to a moving object.
	bool isStationaryAnchor = false;

	// if isStationaryAnchor, then fixed point will be stored here.
	Vector3 globalAnchorPoint;
	// if !isStationaryAnchor, then moving object's transform will be stored here.
	Transform anchorTransform;
	
	SimpleTweetHolder holder;

	void Start() {
		holder = transform.parent.GetComponent<SimpleTweetHolder>();
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
    public void OnInputClicked(InputClickedEventData eventData) {
        StartCoroutine(holder.MaximiseTweet());
    }
}
