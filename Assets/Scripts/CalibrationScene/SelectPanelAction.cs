using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class SelectPanelAction : MonoBehaviour, IInputClickHandler {

	public GameObject fixedPanel;
	private TargetsManager targetsManager;

    // Use this for initialization
    void Start () {
		targetsManager = GameObject.Find("TargetsManager").GetComponent<TargetsManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private GameObject InstantiateAugmentedPanel(GameObject originalPanel) {
		GameObject panelClone = GameObject.Instantiate(originalPanel);
		panelClone.transform.SetParent(originalPanel.transform.parent.parent);
		panelClone.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		panelClone.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
		UnityEngine.Object.Destroy(panelClone.GetComponent<SelectPanelAction>());
		Debug.Log("Instantiated new panel!");
		panelClone.AddComponent<ModifiedBoundingBoxRig>();
		return panelClone;
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {

		Debug.Log(gameObject.transform.lossyScale);
        // InstantiateAugmentedPanel(this.gameObject);
		
		WorldAnchorManager anchorManager = WorldAnchorManager.Instance;

		if (anchorManager == null) {
			Debug.Log("Anchor manager not found, please attach it to scene!");
			return;
		}

		GameObject anchoredClone = null;

		anchoredClone = GameObject.Instantiate(fixedPanel
			, gameObject.transform.position
			, gameObject.transform.rotation);
		anchoredClone.transform.localScale = gameObject.transform.lossyScale;

		anchorManager.AttachAnchor(anchoredClone, gameObject.transform.parent.name);
		anchoredClone.GetComponent<FixedPanel>().RegisterImageTarget(gameObject.transform.parent.gameObject);
    }
}
