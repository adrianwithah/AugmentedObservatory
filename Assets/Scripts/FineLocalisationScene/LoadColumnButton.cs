using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;

public class LoadColumnButton : MonoBehaviour, IInputClickHandler {

	public GameObject columnObjectToAnchor;

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

		LoadAllColumnAnchors();

		ProgressIndicator.Instance.Close();

		gameObject.GetComponent<CompoundButton>().MainCollider.enabled = true;
    }

	void LoadAllColumnAnchors() {
		Debug.Log("Trying to load all column anchors from the store..");
		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return;
		}

		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {
			if (!ids[i].StartsWith(TargetsManager.columnImageTargetPrefix)) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}

			GameObject clone = GameObject.Instantiate(columnObjectToAnchor);
			clone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

			if (store.Load(ids[i], clone) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Column with id: {0}.", ids[i]);
			}

			ProgressIndicator.Instance.SetMessage(string.Format("Checking anchor number {0} out of {1}.", (i + 1), ids.Length));
			ProgressIndicator.Instance.SetProgress((i + 1) / ids.Length);
		}
	}
}
