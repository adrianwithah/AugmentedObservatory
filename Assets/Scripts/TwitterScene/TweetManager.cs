using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetManager : NonPersistSingleton<TweetManager> {

    public Dictionary<int, GameObject> panelNumberToObject = new Dictionary<int, GameObject>();
    public GameObject tweetPrefab;
    public GameObject simpleTweetPrefab;
    public GameObject statsPanelPrefab;

    public void Start() {
        // For now, only column anchors supported. You can easily use panel anchors by changing
        // to the LoadAllPanelAnchorsFromStore method, using the appPanel prefab. Will have to
        // build a way to determine CalibrationMode though.

        OnLoadCompleted(AnchorsManager.Instance.LoadAllColumnAnchorsFromStore(PrefabsManager.Instance.appColumn));

        // Enable the below lines of code in order to test the application in Unity editor.
        // "Holder" gameObjects will simulate the augmented panels in the Observatory.

        // GameObject holder = GameObject.Find("Holder");
        // holder.GetComponent<AppPanel>().SetPanelNumber(6);
        // panelNumberToObject.Add(6, holder);
        // holder = GameObject.Find("Holder2");
        // holder.GetComponent<AppPanel>().SetPanelNumber(7);
        // panelNumberToObject.Add(7, holder);
        // holder = GameObject.Find("Holder3");
        // holder.GetComponent<AppPanel>().SetPanelNumber(22);
        // panelNumberToObject.Add(22, holder);
        // holder = GameObject.Find("Holder4");
        // holder.GetComponent<AppPanel>().SetPanelNumber(23);
        // panelNumberToObject.Add(23, holder);
        // holder = GameObject.Find("Holder5");
        // holder.GetComponent<AppPanel>().SetPanelNumber(38);
        // panelNumberToObject.Add(38, holder);
        // holder = GameObject.Find("Holder6");
        // holder.GetComponent<AppPanel>().SetPanelNumber(39);
        // panelNumberToObject.Add(39, holder);
    }

    private void OnLoadCompleted(Dictionary<string, GameObject> anchorIdToObject) {
        foreach (string anchorId in anchorIdToObject.Keys) {
            int columnNum = TargetsManager.GetColumnNumberFromColumnName(anchorId) + 1;
            GameObject columnObj;
            if (anchorIdToObject.TryGetValue(anchorId, out columnObj)) {
                panelNumberToObject.Add(columnNum, columnObj.transform.GetChild(0).gameObject);
                columnObj.transform.GetChild(0).gameObject.GetComponent<AppPanel>().SetPanelNumber(columnNum);
                panelNumberToObject.Add(columnNum  + Commons.numPanelsPerRow, columnObj.transform.GetChild(1).gameObject);
                columnObj.transform.GetChild(1).gameObject.GetComponent<AppPanel>().SetPanelNumber(columnNum  + Commons.numPanelsPerRow);
                panelNumberToObject.Add(columnNum  + Commons.numPanelsPerRow * 2, columnObj.transform.GetChild(2).gameObject);
                columnObj.transform.GetChild(2).gameObject.GetComponent<AppPanel>().SetPanelNumber(columnNum  + Commons.numPanelsPerRow * 2);
                panelNumberToObject.Add(columnNum  + Commons.numPanelsPerRow * 3, columnObj.transform.GetChild(3).gameObject);
                columnObj.transform.GetChild(3).gameObject.GetComponent<AppPanel>().SetPanelNumber(columnNum  + Commons.numPanelsPerRow * 3);
            }
        }
    }
}
