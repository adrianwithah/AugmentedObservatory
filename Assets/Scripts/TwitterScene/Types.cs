using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Types : MonoBehaviour {

	[SerializeField]
	private Transform retweetedBar;

	[SerializeField]
	private Transform remainBar;

	[SerializeField]
	private	Transform unknownBar;
	[SerializeField]
	private	Transform leaveBar;

	[SerializeField]
	private	Transform allBar;
	[SerializeField]
	private TextMesh retweetedText;

	[SerializeField]
	private TextMesh remainText;

	[SerializeField]
	private	TextMesh unknownText;
	[SerializeField]
	private	TextMesh leaveText;

	[SerializeField]
	private	TextMesh allText;

	private bool ScaledAndPositioned = false;

	public void DisplayGraph(double numRetweeted, double numRemain, double numUnknown
		, double numLeave) {

		if (ScaledAndPositioned) {
			return;
		}

		double numAll = numRetweeted + numRemain + numUnknown + numLeave;

		double XPerValueScale = allBar.localScale.x / numAll;

		ScaleAndPositionBar(retweetedBar, numRetweeted, XPerValueScale);
		retweetedText.text = string.Format("Retweeted\n({0})", numRetweeted);

		ScaleAndPositionBar(remainBar, numRemain, XPerValueScale);
		remainText.text = string.Format("Remain\n({0})", numRemain);

		ScaleAndPositionBar(unknownBar, numUnknown, XPerValueScale);
		unknownText.text = string.Format("Unknown\n({0})", numUnknown);

		ScaleAndPositionBar(allBar, numAll, XPerValueScale);
		allText.text = string.Format("All\n({0})", numAll);

		ScaleAndPositionBar(leaveBar, numLeave, XPerValueScale);
		leaveText.text = string.Format("Leave\n({0})", numLeave);

		ScaledAndPositioned = true;
	}

	private void ScaleAndPositionBar(Transform bar, double value, double valuePerXScale) {
		float newScaleX = (float) (value * valuePerXScale);

		bar.localPosition = new Vector3(bar.localPosition.x - (bar.localScale.x / 2 - newScaleX / 2)
			, bar.localPosition.y, bar.localPosition.z);

		bar.localScale = new Vector3(newScaleX, bar.localScale.y, bar.localScale.z);
	}
}
