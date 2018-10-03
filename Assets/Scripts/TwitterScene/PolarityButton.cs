using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class PolarityButton : MonoBehaviour, IInputClickHandler
{

    private StatsPanel statsPanel;
    void Start() {
        statsPanel = transform.parent.GetComponent<StatsPanel>();
    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        statsPanel.DisplayPolarity();
    }
}
