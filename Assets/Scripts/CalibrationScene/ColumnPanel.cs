using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class ColumnPanel : MonoBehaviour, IInputClickHandler {

	private TargetsManager targetsManager;

    // Use this for initialization
    void Start () {
		targetsManager = GameObject.Find("TargetsManager").GetComponent<TargetsManager>();

		if (targetsManager == null) {
			Debug.Log("Cannot find targetsManager!");
		}
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		
		// This debug statement helps to identify the scale in Unity of the augmented
		// columns and panels which we will need to accurate place the augmentations.

		// Debug.Log("Column Object lossyScale: x: " + transform.parent.lossyScale.x 
		// 	+ " y: " + transform.parent.lossyScale.y 
		// 	+ " z: " + transform.parent.lossyScale.z);
		
		CustomAudioManager.Instance.PlayInputClicked();

		WorldAnchorManager anchorManager = WorldAnchorManager.Instance;

		if (anchorManager == null) {
			Debug.Log("Anchor manager not found, please attach it to scene!");
			return;
		}

		GameObject anchoredClone = null;

		anchoredClone = GameObject.Instantiate(PrefabsManager.Instance.fixedColumn
			, transform.parent.position
			, transform.parent.rotation);
		
		anchoredClone.transform.localScale = transform.parent.lossyScale;

		for (int i = 0; i < anchoredClone.transform.childCount; i++) {
			anchoredClone.transform.GetChild(i).gameObject.GetComponent<FixedColumnPanel>()
				.RegisterImageTarget(gameObject.transform.parent.parent.gameObject);
		}

		anchorManager.AttachAnchor(anchoredClone, gameObject.transform.parent.parent.name);
    }
}
