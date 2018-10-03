using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

// Button which allows users to toggle tagalong behaviour on/off.
public class TagalongButton : MonoBehaviour, IInputClickHandler {

	private CompoundButtonText buttonText;
	private Toolbar toolbar;
	private Tagalong tagalong;

	void Start () {
		toolbar = FindObjectOfType<Toolbar>();
		tagalong = toolbar.EnsureComponent<Tagalong>();
		buttonText = gameObject.GetComponent<CompoundButtonText>();
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (eventData.used) {
			return;
		}
		eventData.Use();
        
		if (tagalong.enabled) {
			tagalong.enabled = false;
			buttonText.Text = "Tagalong off";
		} else {
			tagalong.enabled = true;
			buttonText.Text = "Tagalong on";
		}
    }
}
