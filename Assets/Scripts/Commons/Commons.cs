using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These are some of the hard coded values that are specific to the GDO and
// also include values that will be used across all scenes in the application.
public static class Commons {

	public static float panelResolutionX = 1920.0f;
	public static float panelResolutionY = 1080.0f;

	public static int numPanelsPerRow = 16;
	public static int numPanelsPerCol = 4;

	public static readonly string dataURL = "http://146.169.178.121:8080";

	public static readonly float maxMSspentPerFrame = 1000 / 100;

	// Determines the 0-indexed row number from the given panel number.
	public static int ZeroedRowFromPanelNum(int panelNumber) {
		return (panelNumber - 1) / numPanelsPerRow;
	}

	// Determines the 0-indexed column number from the given panel number.
	public static int ZeroedColFromPanelNum(int panelNumber) {
		return (panelNumber - 1) % numPanelsPerRow;
	}
}