using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTweetHolder : MonoBehaviour {

	Transform maximisedHolder;
	Transform minimisedHolder;

	void Awake() {
		maximisedHolder = transform.Find("Maximised");
		minimisedHolder = transform.Find("Minimised");
	}

	// Use this for initialization
	void Start () {
		maximisedHolder.localScale = new Vector3(0.0f, 0.0f, 0.0f);
		minimisedHolder.localScale = new Vector3(1.0f, 1.0f, 1.0f);	
	}

	public IEnumerator MinimiseTweet() {
		Transform tweet = maximisedHolder.Find("Tweet");
		maximisedHolder.localPosition = maximisedHolder.parent.InverseTransformPoint(tweet.position);
		tweet.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

		Vector3 localStartScale = maximisedHolder.localScale;
		Vector3 localDestScale = new Vector3(0.0f, 0.0f, 0.0f);
		float tweetOpenOverTime = 0.25f;

		float startTime = Time.time;
		while (Time.time - startTime < tweetOpenOverTime) {
			maximisedHolder.localScale = Vector3.Lerp(
				localStartScale, localDestScale, (Time.time - startTime) / tweetOpenOverTime);
            
			yield return null;
		}

		maximisedHolder.localScale = localDestScale;

		minimisedHolder.localPosition = maximisedHolder.localPosition;
		minimisedHolder.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	public IEnumerator MaximiseTweet() {
		minimisedHolder.localScale = new Vector3(0.0f, 0.0f, 0.0f);

		Vector3 localStartScale = maximisedHolder.localScale;
		Vector3 localDestScale = new Vector3(1.0f, 1.0f, 1.0f);
		float tweetOpenOverTime = 0.25f;

		float startTime = Time.time;
		while (Time.time - startTime < tweetOpenOverTime) {
			maximisedHolder.localScale = Vector3.Lerp(
				localStartScale, localDestScale, (Time.time - startTime) / tweetOpenOverTime);
            
			yield return null;
		}

		maximisedHolder.localScale = localDestScale;
	}
}
