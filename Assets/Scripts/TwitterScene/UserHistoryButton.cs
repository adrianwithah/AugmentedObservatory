using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Dialog;
using UnityEngine;
using UnityEngine.Networking;

public class UserHistoryButton : MonoBehaviour, IInputClickHandler
{
	List<GameObject> userTweets = new List<GameObject>(0);
	bool showUserHistory = true;
	private BoxCollider collider;
	private CompoundButtonText buttonText;

	// After a certain threshold, the User History functionality becomes more of a burden,
	// due to instantiating high amounts of GameObjects which creates input delay.
	private readonly int userHistoryThreshold = 300;

	void Start() {
		collider = gameObject.GetComponent<BoxCollider>();
		buttonText = GetComponent<CompoundButtonText>();
	}

    public void OnInputClicked(InputClickedEventData eventData) {
		if (eventData.used) {
			return;
		}
		eventData.Use();

		if (showUserHistory) {
			string username = transform.parent.parent.GetComponent<Tweet>().GetUsername();
			collider.enabled = false;
			StartCoroutine(GetUserNodes(username));
		} else {
			DestroyUserHistory();
			buttonText.Text = "Show History";
		}
		
		showUserHistory = !showUserHistory;
    }

	// Fetches the nodes belonging to a single user. There is sorting before instantiation as an
	// attempt to declutter the augmented tweets. The decluttering process is such that tweets are
	// instantiated in increasing x coordinates. Also, tweets are instantiated in the following
	// cycle: High top, Low bottom, middle top, middle bottom, using varying y offsets.
	IEnumerator GetUserNodes(string username) {
		UnityWebRequest req = UnityWebRequest.Get(string.Format("{0}/user_nodes/?username={1}"
			, Commons.dataURL, username));
		req.SendWebRequest();

		while (!req.isDone) {
			yield return null;
		}
		if (req.isHttpError || req.isNetworkError) {
			Debug.Log(req.error);
			yield break;
		}

		// Stopwatch is used to controll the time spent within each frame. Coroutines are
		// synchronous and we want to leave unfinished work to the next frame once we spend
		// a certain amount of time in one frame. This is to prevent freezing of frames.
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		float frameStartTime = 0.0f;

		TweetDataNode[] nodes = JSONParser.parseJSONObject<UserNodes>(req.downloadHandler.text).Nodes;

		if (nodes.Length > userHistoryThreshold) {
			Dialog.Open(PrefabsManager.Instance.dialogPrefab
				, DialogButtonType.OK
				, "Error"
				, string.Format("Found {0} nodes belonging to {1}. Too many to render."
					, nodes.Length, username));
			yield break;
		}

		Array.Sort(nodes, delegate(TweetDataNode n1, TweetDataNode n2) {
			return n1.Pos.X.CompareTo(n2.Pos.X);
		});

		int counter = 0;
		bool renderAbove = true;
		foreach (TweetDataNode node in nodes) {
			if (sw.ElapsedMilliseconds - frameStartTime >= Commons.maxMSspentPerFrame) {
				yield return null;
				frameStartTime = sw.ElapsedMilliseconds;
			}

			int col = (int) (node.Pos.X / Commons.panelResolutionX);
			int row = (int) (node.Pos.Y / Commons.panelResolutionY);
			int panelNumber = (col + 1) + Commons.numPanelsPerRow * row;

			GameObject panelWithNode;
			if (TweetManager.Instance.panelNumberToObject.TryGetValue(panelNumber, out panelWithNode)) {

				GameObject clone = GameObject.Instantiate(TweetManager.Instance.simpleTweetPrefab, panelWithNode.transform);
				clone.name = counter.ToString();
				clone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
				float yOffset = renderAbove ? 0.5f : -0.5f;
				// yOffset /= (counter % 4 >= 2 ? 2 : 1);
				float localX = (float) (node.Pos.X % Commons.panelResolutionX) / Commons.panelResolutionX - 0.50f;
				float localY = (float) (1 - (node.Pos.Y % Commons.panelResolutionY) / Commons.panelResolutionY) - 0.50f;

				clone.transform.localPosition = new Vector3(localX
					, localY + yOffset
					, -0.8f);
				
				clone.transform.Find("Minimised").GetComponent<MinimisedQuad>().AddAnchor(
					panelWithNode.transform.TransformPoint(new Vector3(localX, localY, 0.0f))
				);

				renderAbove = !renderAbove;

				Tweet tweet = clone.transform.Find("Maximised").GetChild(0).GetComponent<Tweet>();
				tweet.AddAnchor(panelWithNode.transform.TransformPoint(
					new Vector3(localX, localY, 0.0f)
				));
				tweet.SetMessage(node.Attrs.tweet);
				tweet.SetUsername(node.Attrs.username);

				userTweets.Add(clone);
				counter++;
			}
		}

		Debug.Log("Number of nodes rendered: " + counter);
		collider.enabled = true;
		buttonText.Text = "Remove User History";
	}

	public void DestroyUserHistory() {
		foreach (GameObject tweet in userTweets) {
			Destroy(tweet);
		}
		userTweets.Clear();
	}
}
