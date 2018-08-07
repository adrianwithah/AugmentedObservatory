using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class SelectColumnPanelAction : MonoBehaviour, IInputClickHandler {

	public GameObject fixedColumn;
	private TargetsManager targetsManager;

    // Use this for initialization
    void Start () {
		targetsManager = GameObject.Find("TargetsManager").GetComponent<TargetsManager>();

		if (targetsManager == null) {
			Debug.Log("Cannot find targetsManager!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		Debug.LogFormat("Column object lossy scale: {0}", transform.parent.lossyScale);
		WorldAnchorManager anchorManager = WorldAnchorManager.Instance;

		if (anchorManager == null) {
			Debug.Log("Anchor manager not found, please attach it to scene!");
			return;
		}

		GameObject anchoredClone = null;

		anchoredClone = GameObject.Instantiate(fixedColumn
			, transform.parent.position
			, transform.parent.rotation);
		
		anchoredClone.transform.localScale = transform.parent.lossyScale;

		for (int i = 0; i < anchoredClone.transform.childCount; i++) {
			anchoredClone.transform.GetChild(i).gameObject.GetComponent<FixedColumnPanel>().RegisterImageTarget(gameObject.transform.parent.parent.gameObject);
		}

		anchorManager.AttachAnchor(anchoredClone, gameObject.transform.parent.parent.name);
    }
}
