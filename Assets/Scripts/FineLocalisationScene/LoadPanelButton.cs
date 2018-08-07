using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;

public class LoadPanelButton : MonoBehaviour, IInputClickHandler {

	public GameObject panelObjectToAnchor;
	// Use this for initialization
    void Start () {

	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = false;	

		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.None,
                            ProgressStyleEnum.ProgressBar,
                            ProgressMessageStyleEnum.Visible,
                            "Loading Anchors from Anchor Store.");

		LoadAllPanelAnchors();

		ProgressIndicator.Instance.Close();

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }

	void LoadAllPanelAnchors() {

		Debug.Log("Trying to load all panel anchors from the store..");
		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return;
		}

		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {

			if (!ids[i].StartsWith(TargetsManager.panelImageTargetPrefix)) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}

			GameObject clone = GameObject.Instantiate(panelObjectToAnchor
				, panelObjectToAnchor.transform.position
				, panelObjectToAnchor.transform.rotation);
			clone.transform.localScale = new Vector3 (1.1f, 0.6f, 0.2f);

			if (store.Load(ids[i], clone) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Panel with id: {0}", ids[i]);
			}

			ProgressIndicator.Instance.SetMessage(string.Format("Checking anchor number {0} out of {1}.", (i + 1), ids.Length));
			ProgressIndicator.Instance.SetProgress((i + 1) / ids.Length);
		}
	}
}
