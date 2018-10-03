using System.Collections;
using HoloToolkit.Unity;
using UnityEngine;

public class Wallet : NonPersistSingleton<Wallet> {

	private Toolbar toolbar;
	private Tagalong toolbarTagalong;
	private GameObject minimiseButton;
	private GameObject maximiseButton;
	private Vector3 originalLocalScale;
	private int spaceCounter;
	private readonly int NUM_SECTIONS = 3;

	private readonly float packOverTime = 0.2f;
	GameObject panelOne;
	GameObject panelTwo;
	GameObject panelThree;
	Vector3 walletOpenToolbarPosition;

	void Start () {
		toolbar = GetComponentInChildren<Toolbar>();
		toolbarTagalong = toolbar.gameObject.GetComponent<Tagalong>();
		minimiseButton = toolbar.transform.Find("MinimiseButton").gameObject;
		maximiseButton = toolbar.transform.Find("MaximiseButton").gameObject;

		originalLocalScale = transform.localScale;

		panelOne = transform.Find("PanelOne").gameObject;
		panelTwo = transform.Find("PanelTwo").gameObject;
		panelThree = transform.Find("PanelThree").gameObject;

		walletOpenToolbarPosition = toolbar.transform.localPosition;

		StartCoroutine(MinimiseWallet());
	}
	
	public IEnumerator MinimiseWallet() {
		//unparent toolbar.
		toolbar.transform.SetParent(null);

		Vector3 endScale = new Vector3(0, 0, 0);

		float startTime = Time.time;
		while (Time.time - startTime < packOverTime) {
			transform.localScale = Vector3.Lerp(
				originalLocalScale, endScale, (Time.time - startTime) / packOverTime);

			yield return null;
		}
		
		transform.localScale = endScale;

		minimiseButton.SetActive(false);
		maximiseButton.SetActive(true);
		toolbar.PositionActiveButtons();
		
		toolbarTagalong.enabled = true;
	}

	public IEnumerator MaximiseWallet() {
		toolbarTagalong.enabled = false;
		transform.position = toolbar.transform.position;
		transform.rotation = toolbar.transform.rotation;

		Vector3 startScale = new Vector3(0, 0, 0);

		float startTime = Time.time;
		while (Time.time - startTime < packOverTime) {
			transform.localScale = Vector3.Lerp(
				startScale, originalLocalScale, (Time.time - startTime) / packOverTime);

			yield return null;
		}
		
		transform.localScale = originalLocalScale;

		toolbar.transform.SetParent(transform);
		toolbar.transform.localPosition = walletOpenToolbarPosition;

		minimiseButton.SetActive(true);
		maximiseButton.SetActive(false);
		toolbar.PositionActiveButtons();
	}

	// Method to combine the box colliders of the different sections, in order to obtain an
	// accurate box collider encapsulating the whole expanded wallet.
	private void CombineChildBoxColliders() {
		bool hasBounds = false;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		
		for (int i = 0; i < gameObject.transform.childCount; ++i) {
			MeshRenderer childRenderer = gameObject.transform.GetChild(i).GetComponent<MeshRenderer>();
			if (childRenderer != null) {
				if (hasBounds) {
					bounds.Encapsulate(childRenderer.bounds);
				}
				else {
					bounds = childRenderer.bounds;
					hasBounds = true;
				}
			}
		}
		
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		collider.center = bounds.center - gameObject.transform.position;
		collider.size = bounds.size;
	}

	public GameObject MakeSpace() {
		//check the oldest panel and clear it to make space for incoming new data.
		GameObject panel;

		switch (spaceCounter) {
			case 0:
				panel = panelOne;
				break;
			case 1:
				panel = panelTwo;
				break;
			case 2:
				panel = panelThree;
				break;
			default:
				panel = null;
				break;
		}

		spaceCounter = (spaceCounter + 1) % NUM_SECTIONS;

		foreach (Transform child in panel.transform) {
			Destroy(child.gameObject);
		}

		return panel;
	}
}
