using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class FixedPanel : MonoBehaviour, IInputClickHandler {

	private GameObject imageTarget;

	public GameObject ImageTarget {
		get {
			return this.imageTarget;
		}

		private set {
			this.imageTarget = value;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RegisterImageTarget(GameObject imageTarget) {
		this.imageTarget = imageTarget;

		if (imageTarget.activeSelf) {
			imageTarget.SetActive(false);		
		}
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		Debug.Log("Removing world anchor and destroying object!");
		WorldAnchorManager.Instance.RemoveAnchor(imageTarget.name);
		imageTarget.SetActive(true);
		Destroy(gameObject);
    }
}
