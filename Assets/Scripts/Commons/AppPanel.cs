using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AppPanel : MonoBehaviour, IInputClickHandler {

	private BoxCollider collider;

	public void Start() {
		collider = gameObject.GetComponent<BoxCollider>();
	}

	//numbering starts from top left of the Observatory, and increases left to right.
	private int panelNumber;
    public void OnInputClicked(InputClickedEventData eventData) {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		RaycastHit hit = GazeManager.Instance.HitInfo;
        Vector3 localCoords = hit.collider.transform.InverseTransformPoint(hit.point);

		float width = collider.size.x;
		float height = collider.size.y;
		Vector2 imageCoords = new Vector3(
			Commons.panelResolutionX / width * (localCoords.x + collider.size.x / 2)
			, - Commons.panelResolutionY / height * (localCoords.y - collider.size.y / 2));

		GameObject highlighter = GameObject.Find("Quad");
		highlighter.transform.position = hit.point;
		highlighter.transform.rotation = hit.transform.rotation;
		
		DataGrabber.Instance.Grab(panelNumber, imageCoords);
	}

	public void SetPanelNumber(int panelNumber) {
		this.panelNumber = panelNumber;
	}
}
