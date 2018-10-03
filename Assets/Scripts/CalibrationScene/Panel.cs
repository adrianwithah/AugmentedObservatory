using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class Panel : MonoBehaviour, IInputClickHandler {

	private TargetsManager targetsManager;

    // Use this for initialization
    void Start () {
		targetsManager = GameObject.Find("TargetsManager").GetComponent<TargetsManager>();
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		// This debug statement helps to identify the scale in Unity of the augmented
		// columns and panels which we will need to accurate place the augmentations.
		
		// Debug.Log("Panel Object lossyScale: x: " + transform.lossyScale.x 
		// 	+ " y: " + transform.lossyScale.y 
		// 	+ " z: " + transform.lossyScale.z);

		// CustomAudioManager.Instance.PlayInputClicked();

		WorldAnchorManager anchorManager = WorldAnchorManager.Instance;

		if (anchorManager == null) {
			Debug.Log("Anchor manager not found, please attach it to scene!");
			return;
		}

		GameObject anchoredClone = null;

		anchoredClone = GameObject.Instantiate(PrefabsManager.Instance.fixedPanel
			, gameObject.transform.position
			, gameObject.transform.rotation);
		anchoredClone.transform.localScale = gameObject.transform.lossyScale;

		anchorManager.AttachAnchor(anchoredClone, gameObject.transform.parent.name);
		anchoredClone.GetComponent<FixedPanel>().RegisterImageTarget(gameObject.transform.parent.gameObject);
    }
}
