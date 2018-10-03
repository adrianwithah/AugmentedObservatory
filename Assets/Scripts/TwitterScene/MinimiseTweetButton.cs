using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MinimiseTweetButton : MonoBehaviour, IInputClickHandler {

	SimpleTweetHolder holder;

	void Awake() {
		holder = transform.parent.parent.parent.parent.GetComponent<SimpleTweetHolder>();
	}

    public void OnInputClicked(InputClickedEventData eventData) {
        StartCoroutine(holder.MinimiseTweet());
    }
}
