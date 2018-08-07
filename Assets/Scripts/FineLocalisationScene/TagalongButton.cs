using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TagalongButton : MonoBehaviour {

	private bool tagalongOn = true;
	private CompoundButtonText buttonText;
	private Toolbar toolbar;

	void Start () {
		toolbar = GameObject.Find("Toolbar").GetComponent<Toolbar>();
		buttonText = gameObject.GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonText>();
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
		if (tagalongOn) {
			toolbar.PauseTagalong();
			buttonText.Text = "Tagalong off";
		} else {
			toolbar.ResumeTagalong();
			buttonText.Text = "Tagalong on";
		}

		tagalongOn = !tagalongOn;
    }
}
