using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible for populating the user's polarity graphs.
public class Polarity : MonoBehaviour {

	[SerializeField]
	private Transform aggrAvgBar;

	[SerializeField]
	private Transform aggrMaxBar;

	[SerializeField]
	private Transform aggrMinBar;

	[SerializeField]
	private	Transform userMaxBar;

	[SerializeField]
	private	Transform userAvgBar;

	[SerializeField]
	private	Transform userMinBar;

	private bool ScaledAndPositioned = false;

	public void ScaleAndPositionBar(Transform bar, double value, double valueToYScale) {
		double difference = value - GetYCutoff();
		difference = difference < 0 ? -difference : difference;

		bar.localScale = new Vector3(bar.localScale.x
			, (float) (difference * valueToYScale)
			, bar.localScale.z);

		double yOffset = value < GetYCutoff() ? bar.localScale.y / 2 : -bar.localScale.y / 2;

		bar.localPosition = new Vector3(bar.localPosition.x
			, (float) (bar.localPosition.y + yOffset)
			, bar.localPosition.z);
	}

	//aggrMax corresponds to the highest Y value (Bottom most point in panel coordinates)
	private double GetAbsoluteMax(double aggrMax, double aggrMin) {
		double cutoff = GetYCutoff();
		return ((aggrMax - cutoff) > (cutoff - aggrMin)) ? aggrMax : aggrMin;
	}

	private double GetYCutoff() {
		return Commons.panelResolutionY * Commons.numPanelsPerCol / 2;
	}

	public void DisplayGraph(double aggrMax, double aggrMin, double aggrAvg
		, double userMax, double userMin, double userAvg) {
		
		if (ScaledAndPositioned) {
			return;
		}

		double difference = GetAbsoluteMax(aggrMax, aggrMin) - GetYCutoff();
		difference = difference < 0 ? -difference : difference;

		double YPerValueScale = aggrMaxBar.localScale.y / difference;

		ScaleAndPositionBar(aggrMaxBar, aggrMax, YPerValueScale);

		ScaleAndPositionBar(aggrMinBar, aggrMin, YPerValueScale);

		ScaleAndPositionBar(aggrAvgBar, aggrAvg, YPerValueScale);

		ScaleAndPositionBar(userMaxBar, userMax, YPerValueScale);

		ScaleAndPositionBar(userAvgBar, userAvg, YPerValueScale);

		ScaleAndPositionBar(userMinBar, userMin, YPerValueScale);

		ScaledAndPositioned = true;
	}
}
