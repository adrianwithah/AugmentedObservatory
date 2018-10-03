using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TweetNode : MonoBehaviour, IInputClickHandler {
    
    private readonly float tweetOpenOverTime = 0.25f;

    public TweetDataNode data;
    public void OnInputClicked(InputClickedEventData eventData) {
        GameObject clone = GameObject.Instantiate(TweetManager.Instance.tweetPrefab, transform.parent);

        // utilise normal of the "Wallet" section it is on in order to instantiate the
        // augmented tweet in the correct rotation.
        Vector3 localDestPos = transform.localPosition + 0.20f * transform.parent.gameObject.GetComponent<MeshFilter>().mesh.normals[0];

        Tweet tweet = clone.GetComponent<Tweet>();
        tweet.SetUsername(data.Attrs.username);
        tweet.SetMessage(data.Attrs.tweet);

        tweet.AddAnchor(transform);

        StartCoroutine(AnimateOpen(clone
            , transform.localPosition
            , new Vector3(0.0f, 0.0f, 0.0f)
            , localDestPos
            , TweetManager.Instance.tweetPrefab.transform.localScale));
    }

    IEnumerator AnimateOpen(GameObject tweet
        , Vector3 localStartPos, Vector3 localStartScale
        , Vector3 localDestPos, Vector3 localDestScale) {

        tweet.transform.localScale = localStartScale;
		tweet.transform.localPosition = localDestPos;

		float startTime = Time.time;
		while (Time.time - startTime < tweetOpenOverTime) {
			tweet.transform.localScale = Vector3.Lerp(
				localStartScale, localDestScale, (Time.time - startTime) / tweetOpenOverTime);
            
            tweet.transform.localPosition = Vector3.Lerp(
                localStartPos, localDestPos, (Time.time - startTime) / tweetOpenOverTime);

			yield return null;
		}
		
		tweet.transform.localScale = localDestScale;
        tweet.transform.localPosition = localDestPos;
    }
}
