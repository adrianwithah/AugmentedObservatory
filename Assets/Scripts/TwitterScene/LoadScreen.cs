// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

// Loading screen script that will utilise the rotating orbs animation. Attached to
// Loading Screen prefab.
public class LoadScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject defaultOrbsPrefab;

	// The message text used by the 'Visible' message style
	[SerializeField]
	private TextMesh messageText;

	[SerializeField]
	private Animator animator;

	GameObject orbs;

	private bool closing = false;

	/// <summary>
	/// Opens the dialog with full custom options
	/// </summary>
	/// <param name="indicatorStyle"></param>
	/// <param name="progressStyle"></param>
	/// <param name="messageStyle"></param>
	/// <param name="message"></param>
	/// <param name="icon"></param>

	void Start() {
		// Make sure we aren't parented under anything
		transform.parent = null;

		// Turn our common objects on 
		closing = false;
		gameObject.SetActive(true);
		messageText.text = "Loading";

		if (defaultOrbsPrefab != null)
		{
			orbs = GameObject.Instantiate(defaultOrbsPrefab);
			orbs.transform.localPosition = new Vector3(0.0f, 25.0f, 0.0f);
			//instantiatedCustomObject.transform.localRotation = Quaternion.identity;
			orbs.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

			orbs.transform.Translate(messageText.transform.position);
			orbs.transform.SetParent(messageText.transform, false);
		}

		animator.SetTrigger("Open");
	}

	/// <summary>
	/// Updates message.
	/// Has no effect until Open is called.
	/// </summary>
	/// <param name="message"></param>
	public void SetMessage(string message)
	{
		if (!gameObject.activeSelf) { return; }

		messageText.text = message;
	}

	/// <summary>
	/// Initiates the process of closing the dialog.
	/// </summary>
	public void Close()
	{
		if (!gameObject.activeSelf) { return; }

		closing = true;
		messageText.gameObject.SetActive(false);
		animator.SetTrigger("Close");
	}

	private void Update()
	{
		// If we're closing, wait for the animator to reach the closed state
		if (closing)
		{
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
			{
				// Once we've reached the cloesd state shut down completely
				closing = false;
				transform.parent = null;
				gameObject.SetActive(false);

				// Destroy our custom object if we made one
				if (orbs != null)
				{
					GameObject.Destroy(orbs);
				}
			}
		}
	}
}
