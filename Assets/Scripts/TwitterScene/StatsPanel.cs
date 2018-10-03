using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;

public class StatsPanel : MonoBehaviour, IManipulationHandler {
	UserStats userStats;
	AggregateStats aggrStats;
	[SerializeField]
	private Polarity polarity;
	[SerializeField]
	private Types types;
    private bool isManipulationEnabled = true;
    private Vector3 manipulationOriginalPosition;

	private string username;

    // Use this for initialization
    void Start () {
		polarity = transform.Find("Polarity").GetComponent<Polarity>();
		types = transform.Find("Types").GetComponent<Types>();
	}

	public void DisplayPolarity() {
		types.gameObject.SetActive(false);

		polarity.DisplayGraph(aggrStats.Highest, aggrStats.Lowest, aggrStats.Average
			, userStats.Max_Y, userStats.Min_Y, userStats.Avg_Y);

		polarity.gameObject.SetActive(true);
	}

	public void DisplayTypes() {
		polarity.gameObject.SetActive(false);

		types.DisplayGraph(userStats.Num_Retweeted, userStats.Num_Remain
			, userStats.Num_Unknown, userStats.Num_Leave);
		
		types.gameObject.SetActive(true);
	}

	// Fetches the user stats data from the server, before populating the polarity graph.
	// Fetched user stats are cached for easy switching between polarity and types view.
	public IEnumerator Initialise(string username) {
			
		this.username = username;

		CompoundButton polarityBtn = transform.Find("PolarityButton").GetComponent<CompoundButton>();
		CompoundButton typesBtn = transform.Find("TypesButton").GetComponent<CompoundButton>();
		polarityBtn.MainCollider.enabled = false;
		typesBtn.MainCollider.enabled = false;

		transform.Find("Username").GetComponent<TextMesh>().text = "@" + username;

		UnityWebRequest req = UnityWebRequest.Get(string.Format("{0}/user_stats/?username={1}"
			, Commons.dataURL, username));
		req.SendWebRequest();

		while (!req.isDone) {
			yield return null;
		}
		if (req.isHttpError || req.isNetworkError) {
			Debug.Log(req.error);
			yield break;
		}

		TweetStats stats = JSONParser.parseJSONObject<TweetStats>(req.downloadHandler.text);
		userStats = stats.User_Stats;
		aggrStats = stats.Aggregate_Stats;

		DisplayPolarity();

		polarityBtn.MainCollider.enabled = true;
		typesBtn.MainCollider.enabled = true;
	}

	void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
    {	
        if (isManipulationEnabled)
        {
			if (eventData.used) {
				return;
			}
			eventData.Use();

            InputManager.Instance.PushModalInputHandler(gameObject);

            manipulationOriginalPosition = transform.position;
        }
    }

    void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (isManipulationEnabled)
        {
			if (eventData.used) {
				return;
			}
			eventData.Use();

            transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
        }
    }

    void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }
}
