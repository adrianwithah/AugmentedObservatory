using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class FixedColumnPanel : MonoBehaviour, IInputClickHandler {
	
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
		Debug.LogFormat("Registering imageTarget: {0}.", imageTarget.name);

		if (imageTarget.activeSelf) {
			Debug.LogFormat("Setting imageTarget: {0} off.", imageTarget.name);
			imageTarget.SetActive(false);		
		}
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		WorldAnchorManager.Instance.RemoveAnchor(imageTarget.name);
		imageTarget.SetActive(true);
		Destroy(transform.parent.gameObject);
    }
}
