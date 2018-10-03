using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class UserStatsButton : MonoBehaviour, IInputClickHandler {

    private readonly float statsPanelOpenOverTime = 0.25f;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (eventData.used) {
            return;
        }

        eventData.Use();

        GameObject statsObj = GameObject.Instantiate(TweetManager.Instance.statsPanelPrefab
            , Wallet.Instance.transform);

		Vector3 localDestPos = new Vector3(0.0f, 0.0f, -0.50f);

        string username = transform.parent.parent.GetComponent<Tweet>().GetUsername();
        StartCoroutine(statsObj.GetComponent<StatsPanel>().Initialise(username));
        StartCoroutine(AnimateOpen(statsObj
            , Wallet.Instance.transform.InverseTransformPoint(transform.position)
            , new Vector3(0.0f, 0.0f, 0.0f)
            , localDestPos
            , TweetManager.Instance.statsPanelPrefab.transform.localScale));
    }

    IEnumerator AnimateOpen(GameObject statsPanel
        , Vector3 localStartPos, Vector3 localStartScale
        , Vector3 localDestPos, Vector3 localDestScale) {

        statsPanel.transform.localScale = localStartScale;
		statsPanel.transform.localPosition = localDestPos;

		float startTime = Time.time;
		while (Time.time - startTime < statsPanelOpenOverTime) {
			statsPanel.transform.localScale = Vector3.Lerp(
				localStartScale, localDestScale, (Time.time - startTime) / statsPanelOpenOverTime);
            
            statsPanel.transform.localPosition = Vector3.Lerp(
                localStartPos, localDestPos, (Time.time - startTime) / statsPanelOpenOverTime);

			yield return null;
		}
		
		statsPanel.transform.localScale = localDestScale;
        statsPanel.transform.localPosition = localDestPos;
    }
}
