using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MinimiseButton : MonoBehaviour, IInputClickHandler {
	public void OnInputClicked(InputClickedEventData eventData)
	{
		if (eventData.used) {
			return;
		}
		eventData.Use();
		
		StartCoroutine(Wallet.Instance.MinimiseWallet());
	}
}
