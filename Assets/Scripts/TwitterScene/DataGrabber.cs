using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataGrabber : NonPersistSingleton<DataGrabber> {

	[SerializeField]
	private GameObject nodeObj;
	[SerializeField]
	private GameObject linkObj;
	private readonly static float pixelDeviation = 150.0f;

	// Utilises bit manipulation as boolean flags. Determines which other panels you have to
	// grab data from before calling the suited method.
	public void Grab(int panelTapped, Vector2 coordTapped) {
		int cumulativeBits = 0;

		// Bit for involving top panel
		if (coordTapped.y - pixelDeviation < 0 && panelTapped > Commons.numPanelsPerRow) {
			cumulativeBits += 1 << 3;
		}

		// Bit for involving left panel
		if (coordTapped.x - pixelDeviation < 0 
			&& (panelTapped - 1) % Commons.numPanelsPerRow > 0) {
			cumulativeBits += 1 << 2;
		}

		// Bit for involving bottom panel
		if (coordTapped.y + pixelDeviation > Commons.panelResolutionY 
			&& panelTapped < Commons.numPanelsPerRow * (Commons.numPanelsPerCol - 1)) {
			cumulativeBits += 1 << 1;
		}

		// Bit for involving right panel
		if (coordTapped.x + pixelDeviation > Commons.panelResolutionX
			&& (panelTapped - 1) % Commons.numPanelsPerRow < (Commons.numPanelsPerRow - 1)) {
			cumulativeBits += 1 << 0;
		}

		switch (cumulativeBits) {
			//0000
			case 0:
				HandleGrabAlone(panelTapped, coordTapped);
				break;
			//0001
			case 1:
				HandleGrabWithRight(panelTapped, coordTapped);
				break;
			//0010
			case 2:
				HandleGrabWithBottom(panelTapped, coordTapped);
				break;
			//0011
			case 3:
				HandleGrabWithBottomAndRight(panelTapped, coordTapped);
				break;
			//0100
			case 4:
				HandleGrabWithLeft(panelTapped, coordTapped);
				break;
			//0101
			case 5:
				Debug.Log("Unlikely you want to grab both left and right...");
				break;
			//0110
			case 6:
				HandleGrabWithBottomAndLeft(panelTapped, coordTapped);
				break;
			//0111
			case 7:
				Debug.Log("Unlikely you want to grab left right AND bottom");
				break;
			//1000
			case 8:
				HandleGrabWithTop(panelTapped, coordTapped);
				break;
			//1001
			case 9:
				HandleGrabWithTopAndRight(panelTapped, coordTapped);
				break;
			//1010
			case 10:
				Debug.Log("Unlikely you want to grab both top and bottom....");
				break;
			//1011
			case 11:
				Debug.Log("Unlikely you want to grab top, bottom and right...");
				break;
			//1100
			case 12:
				HandleGrabWithTopAndLeft(panelTapped, coordTapped);
				break;
			//1101
			case 13:
				Debug.Log("Unlikely you want to grab top, left and right");
				break;
			//1110
			case 14:
				Debug.Log("Unlike you want to grab top, left and bottom");
				break;
			//1111
			case 15:
				Debug.Log("Unlikely you want to grab all sides.");
				break;
			default:
				Debug.Log("Invalid flags");
				break;
		}
	}

	// Based on the combinations of panels for which we grab data for, we call the
	// appropriate HandleGrab method. In the method, we determine the panel number and exact 
	// coordinates (in terms of each panel's coordinates) of each panel from which we grab data.
	private void HandleGrabAlone(int panelTapped, Vector2 coordTapped) {
		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation);
		
		StartCoroutine(GetJSON(new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight)));
	}

	private void HandleGrabWithTop(int panelTapped, Vector2 coordTapped) {
		Vector2 topPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, Commons.panelResolutionY + (coordTapped.y - pixelDeviation));
		Vector2 topPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, Commons.panelResolutionY);
		DataRect topRect = new DataRect(panelTapped - Commons.numPanelsPerRow
			,topPanelTopLeft, topPanelBottomRight);

		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation, 0);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation);
		DataRect bottomRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		StartCoroutine(GetJSONTopBottom(topRect, bottomRect));
	}

	private void HandleGrabWithTopAndLeft(int panelTapped, Vector2 coordTapped) {
		Vector2 topLeftPanelTopLeft = new Vector2(Commons.panelResolutionX + (coordTapped.x - pixelDeviation)
			, Commons.panelResolutionY + (coordTapped.y - pixelDeviation));
		Vector2 topLeftPanelBottomRight = new Vector2(Commons.panelResolutionX, Commons.panelResolutionY);
		DataRect topLeftRect = new DataRect(panelTapped - Commons.numPanelsPerRow - 1
			,topLeftPanelTopLeft, topLeftPanelBottomRight);

		Vector2 topPanelTopLeft = new Vector2(0, Commons.panelResolutionY + (coordTapped.y - pixelDeviation));
		Vector2 topPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation, Commons.panelResolutionY);
		DataRect topRightRect = new DataRect(panelTapped - Commons.numPanelsPerRow
			, topPanelTopLeft, topPanelBottomRight);

		Vector2 leftPanelTopLeft = new Vector2(Commons.panelResolutionX + (coordTapped.x - pixelDeviation), 0);
		Vector2 leftPanelBottomRight = new Vector2(Commons.panelResolutionX, coordTapped.y + pixelDeviation);
		DataRect bottomLeftRect = new DataRect(panelTapped - 1
			, leftPanelTopLeft, leftPanelBottomRight);

		Vector2 currPanelTopLeft = new Vector2(0, 0);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation);
		DataRect bottomRightRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		StartCoroutine(GetJSON(topLeftRect, topRightRect, bottomRightRect, bottomLeftRect));
	}

	private void HandleGrabWithTopAndRight(int panelTapped, Vector2 coordTapped) {
		Vector2 topPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, Commons.panelResolutionY + coordTapped.y - pixelDeviation);
		Vector2 topPanelBottomRight = new Vector2(Commons.panelResolutionX, Commons.panelResolutionY);
		DataRect topLeftRect = new DataRect(panelTapped - Commons.numPanelsPerRow
			, topPanelTopLeft, topPanelBottomRight);

		Vector2 topRightPanelTopLeft = new Vector2(0, Commons.panelResolutionY + (coordTapped.y - pixelDeviation));
		Vector2 topRightPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation - Commons.panelResolutionX
			, Commons.panelResolutionY);
		DataRect topRightRect = new DataRect(panelTapped - Commons.numPanelsPerRow + 1
			, topRightPanelTopLeft, topRightPanelBottomRight);
	
		Vector2 rightPanelTopLeft = new Vector2(0, 0);
		Vector2 rightPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation - Commons.panelResolutionX
			, coordTapped.y + pixelDeviation);
		DataRect bottomRightRect = new DataRect(panelTapped + 1
			, rightPanelTopLeft, rightPanelBottomRight);

		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation, 0);
		Vector2 currPanelBottomRight = new Vector2(Commons.panelResolutionX, coordTapped.y + pixelDeviation);
		DataRect bottomLeftRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);
		//send requests

		StartCoroutine(GetJSON(topLeftRect, topRightRect, bottomRightRect, bottomLeftRect));
	}

	private void HandleGrabWithRight(int panelTapped, Vector2 coordTapped) {
		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(Commons.panelResolutionX, coordTapped.y + pixelDeviation);
		DataRect leftRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		Vector2 rightPanelTopLeft = new Vector2(0, coordTapped.y - pixelDeviation);
		Vector2 rightPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation - Commons.panelResolutionX
			, coordTapped.y + pixelDeviation);
		DataRect rightRect = new DataRect(panelTapped + 1, rightPanelTopLeft, rightPanelBottomRight);

		StartCoroutine(GetJSONLeftRight(leftRect, rightRect));
	}

	private void HandleGrabWithLeft(int panelTapped, Vector2 coordTapped) {
		Vector2 leftPanelTopLeft = new Vector2(Commons.panelResolutionX + coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 leftPanelBottomRight = new Vector2(Commons.panelResolutionX, coordTapped.y + pixelDeviation);
		DataRect leftRect = new DataRect(panelTapped - 1, leftPanelTopLeft, leftPanelBottomRight);

		Vector2 currPanelTopLeft = new Vector2(0, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation);
		DataRect rightRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		StartCoroutine(GetJSONLeftRight(leftRect, rightRect));
	}

	private void HandleGrabWithBottom(int panelTapped, Vector2 coordTapped) {
		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, Commons.panelResolutionY);
		DataRect topRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		Vector2 bottomPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation, 0);
		Vector2 bottomPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation - Commons.panelResolutionY);
		DataRect bottomRect = new DataRect(panelTapped + Commons.numPanelsPerRow
			, bottomPanelTopLeft, bottomPanelBottomRight);

		StartCoroutine(GetJSONTopBottom(topRect, bottomRect));
	}
	private void HandleGrabWithBottomAndLeft(int panelTapped, Vector2 coordTapped) {
		Vector2 leftPanelTopLeft = new Vector2(Commons.panelResolutionX + coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 leftPanelBottomRight = new Vector2(Commons.panelResolutionX, Commons.panelResolutionY);
		DataRect topLeftRect = new DataRect(panelTapped - 1, leftPanelTopLeft, leftPanelBottomRight);

		Vector2 currPanelTopLeft = new Vector2(0, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation, Commons.panelResolutionY);
		DataRect topRightRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		Vector2 bottomPanelTopLeft = new Vector2(0, 0);
		Vector2 bottomPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation
			, coordTapped.y + pixelDeviation - Commons.panelResolutionY);
		DataRect bottomRightRect = new DataRect(panelTapped + Commons.numPanelsPerRow
			, bottomPanelTopLeft, bottomPanelBottomRight);

		Vector2 bottomLeftPanelTopLeft
			= new Vector2(Commons.panelResolutionX + coordTapped.x - pixelDeviation, 0);
		Vector2 bottomLeftPanelBottomRight = new Vector2(Commons.panelResolutionX
			, coordTapped.y + pixelDeviation - Commons.panelResolutionY);
		DataRect bottomLeftRect = new DataRect(panelTapped + Commons.numPanelsPerRow - 1
			, bottomLeftPanelTopLeft, bottomLeftPanelBottomRight);

		StartCoroutine(GetJSON(topLeftRect, topRightRect, bottomRightRect, bottomLeftRect));
	}
	private void HandleGrabWithBottomAndRight(int panelTapped, Vector2 coordTapped) {
		Vector2 currPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation
			, coordTapped.y - pixelDeviation);
		Vector2 currPanelBottomRight = new Vector2(Commons.panelResolutionX, Commons.panelResolutionY);
		DataRect topLeftRect = new DataRect(panelTapped, currPanelTopLeft, currPanelBottomRight);

		Vector2 rightPanelTopLeft = new Vector2(0, coordTapped.y - pixelDeviation);
		Vector2 rightPanelBottomRight = new Vector2(coordTapped.x + pixelDeviation - Commons.panelResolutionX
			, Commons.panelResolutionY);
		DataRect topRightRect = new DataRect(panelTapped + 1
			, rightPanelTopLeft, rightPanelBottomRight);
		
		Vector2 bottomRightPanelTopLeft = new Vector2(0, 0);
		Vector2 bottomRightPanelBottomRight = new Vector2(
				coordTapped.x + pixelDeviation - Commons.panelResolutionX
				, coordTapped.y + pixelDeviation - Commons.panelResolutionY);
		DataRect bottomRightRect = new DataRect(panelTapped + Commons.numPanelsPerRow + 1
			, bottomRightPanelTopLeft, bottomRightPanelBottomRight);
		
		Vector2 bottomPanelTopLeft = new Vector2(coordTapped.x - pixelDeviation, 0);
		Vector2 bottomPanelBottomRight = new Vector2(Commons.panelResolutionX
			, coordTapped.y + pixelDeviation - Commons.panelResolutionY);
		DataRect bottomLeftRect = new DataRect(panelTapped + Commons.numPanelsPerRow
			, bottomPanelTopLeft, bottomPanelBottomRight);
		
		StartCoroutine(GetJSON(topLeftRect, topRightRect, bottomRightRect, bottomLeftRect));
	}

	// After determining the exact rectangles for each panel to grab data from, we
	// make the actual GET requests. This is done using Coroutines which check for
	// async operation completion every frame.
	IEnumerator GetJSON(DataRect rect) {
		UnityWebRequest req = SendRequest(rect);

		while (!req.isDone) {
			yield return null;
		}
		if (req.isNetworkError || req.isHttpError) {
			Debug.Log(req.error);
			yield break;
		}
		
		GameObject panel = Wallet.Instance.MakeSpace();

		TweetData results = JSONParser.parseJSONObject<TweetData>(req.downloadHandler.text);

		yield return StartCoroutine(PlaceResultsInSection(results, -rect.topLeft.x, -rect.topLeft.y, panel, rect.panelNumber));
	}

	// See GetJSON method for explanation.
	IEnumerator GetJSONLeftRight(DataRect leftRect, DataRect rightRect) {
		UnityWebRequest reqLeft = SendRequest(leftRect);
		UnityWebRequest reqRight = SendRequest(rightRect);

		while (!reqLeft.isDone) {
			yield return null;
		}
		if (reqLeft.isNetworkError || reqLeft.isHttpError) {
			Debug.Log(reqLeft.error);
			yield break;
		}

		while (!reqRight.isDone) {
			yield return null;
		}
		if (reqRight.isNetworkError || reqRight.isHttpError) {
			Debug.Log(reqRight.error);
			yield break;
		}

		GameObject panel = Wallet.Instance.MakeSpace();

		TweetData leftResults = JSONParser.parseJSONObject<TweetData>(reqLeft.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(leftResults, -leftRect.topLeft.x, -leftRect.topLeft.y, panel, leftRect.panelNumber));

		TweetData rightResults = JSONParser.parseJSONObject<TweetData>(reqRight.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(rightResults, pixelDeviation * 2 - rightRect.bottomRight.x
			, -rightRect.topLeft.y, panel, rightRect.panelNumber));
	}

	// See GetJSON method for explanation.
	IEnumerator GetJSONTopBottom(DataRect topRect, DataRect bottomRect) {
		UnityWebRequest reqTop = SendRequest(topRect);
		UnityWebRequest reqBottom = SendRequest(bottomRect);

		while (!reqTop.isDone) {
			yield return null;
		}
		if (reqTop.isNetworkError || reqTop.isHttpError) {
			Debug.Log(reqTop.error);
			yield break;
		}

		while (!reqBottom.isDone) {
			yield return null;
		}
		if (reqBottom.isNetworkError || reqBottom.isHttpError) {
			Debug.Log(reqBottom.error);
			yield break;
		}

		GameObject panel = Wallet.Instance.MakeSpace();

		TweetData topResults = JSONParser.parseJSONObject<TweetData>(reqTop.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(topResults, -topRect.topLeft.x, -topRect.topLeft.y, panel, topRect.panelNumber));

		TweetData bottomResults 
			= JSONParser.parseJSONObject<TweetData>(reqBottom.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(bottomResults, -bottomRect.topLeft.x
			, pixelDeviation * 2 - bottomRect.bottomRight.y, panel, bottomRect.panelNumber));
	}

	// See GetJSON method for explanation.
	IEnumerator GetJSON(DataRect topLeftRect, DataRect topRightRect, DataRect bottomRightRect
		, DataRect bottomLeftRect) {

		UnityWebRequest reqTopLeft = SendRequest(topLeftRect);
		UnityWebRequest reqTopRight = SendRequest(topRightRect);
		UnityWebRequest reqBottomRight = SendRequest(bottomRightRect);
		UnityWebRequest reqBottomLeft = SendRequest(bottomLeftRect);

		while (!reqTopLeft.isDone) {
			yield return null;
		}
		if (reqTopLeft.isNetworkError || reqTopLeft.isHttpError) {
			Debug.Log(reqTopLeft.error);
			yield break;
		}

		while (!reqTopRight.isDone) {
			yield return null;
		}
		if (reqTopRight.isNetworkError || reqTopRight.isHttpError) {
			Debug.Log(reqTopRight.error);
			yield break;
		}

		while (!reqBottomRight.isDone) {
			yield return null;
		}
		if (reqBottomRight.isNetworkError || reqBottomRight.isHttpError) {
			Debug.Log(reqBottomRight.error);
			yield break;
		}

		while (!reqBottomLeft.isDone) {
			yield return null;
		}
		if (reqBottomLeft.isNetworkError || reqBottomLeft.isHttpError) {
			Debug.Log(reqBottomLeft.error);
			yield break;
		}

		GameObject panel = Wallet.Instance.MakeSpace();

		List<Action> actions = new List<Action>();

		TweetData results = JSONParser.parseJSONObject<TweetData>(reqTopLeft.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(results, -topLeftRect.topLeft.x, -topLeftRect.topLeft.y, panel, topLeftRect.panelNumber));

		Debug.Log("Placed top left!");

		results = JSONParser.parseJSONObject<TweetData>(reqTopRight.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(results, pixelDeviation * 2 - topRightRect.bottomRight.x
			, -topRightRect.topLeft.y, panel, topRightRect.panelNumber));

		Debug.Log("Placed top right!");
		
		results = JSONParser.parseJSONObject<TweetData>(reqBottomLeft.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(results, -bottomLeftRect.topLeft.x
			, pixelDeviation * 2 - bottomLeftRect.bottomRight.y, panel, bottomLeftRect.panelNumber));
		
		Debug.Log("Placed bottom left!");
	
		results = JSONParser.parseJSONObject<TweetData>(reqBottomRight.downloadHandler.text);
		yield return StartCoroutine(PlaceResultsInSection(results, pixelDeviation * 2 - bottomRightRect.bottomRight.x
			, pixelDeviation * 2 - bottomRightRect.bottomRight.y, panel, bottomRightRect.panelNumber));
		
		Debug.Log("Placed bottom right!");
	}

	// Once data has been fetched using the GET requests and we have been allocated
	// a space in the "Wallet" to populate, we have to instantiate the nodes and links.
	// Before instantiating, we have to translate the nodes such that the nodes fit accurately
	// in the "Wallet" section.

	// Translation order:
	// Image coordinates -> Coordinates within pixelDeviation rect 
	// -> Coordinates within section object.
	IEnumerator PlaceResultsInSection(TweetData results, float translationX
		, float translationY, GameObject panel, int panelNumber) {

		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		float frameStartTime = 0.0f;

		foreach (TweetDataNode node in results.Nodes) {
			if (sw.ElapsedMilliseconds - frameStartTime >= Commons.maxMSspentPerFrame) {
				yield return null;
				frameStartTime = sw.ElapsedMilliseconds;
			}

			node.Translate(translationX, translationY);
			PlaceNodeInSection(node, panelNumber, panel);	
		}
		foreach (TweetDataLink link in results.Links) {
			if (sw.ElapsedMilliseconds - frameStartTime >= Commons.maxMSspentPerFrame) {
				yield return null;
				frameStartTime = sw.ElapsedMilliseconds;
			}

			link.Translate(translationX, translationY);
			PlaceLinkInSection(link, panelNumber, panel);
		}
	}

	private void PlaceNodeInSection(TweetDataNode node, int panelNumber, GameObject section) {
		int row = Commons.ZeroedRowFromPanelNum(panelNumber);
		int col = Commons.ZeroedColFromPanelNum(panelNumber);

		GameObject clone = GameObject.Instantiate(nodeObj, section.transform);
		clone.GetComponent<Renderer>().material.color = new Color32((byte) node.R
			, (byte) node.G
			, (byte) node.B
			, 255);
		clone.GetComponent<TweetNode>().data = node;
		clone.transform.localPosition = new Vector3(
			(float) ((node.Pos.X - col * Commons.panelResolutionX) / (pixelDeviation * 2) - 0.50f)
			, (float) (1 - ((node.Pos.Y - row * Commons.panelResolutionY) / (pixelDeviation * 2)) - 0.50f)
			, 0.0f);
	}

	private void PlaceLinkInSection(TweetDataLink link, int panelNumber, GameObject section) {
		// Cylinder GameObject chosen instead of LineRenderer, for the 3D effect.
		
		int row = Commons.ZeroedRowFromPanelNum(panelNumber);
		int col = Commons.ZeroedColFromPanelNum(panelNumber);

		GameObject cylinderClone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		Color32 linkColor = new Color32((byte) link.R, (byte) link.G, (byte) link.B, 255);
		cylinderClone.GetComponent<Renderer>().material.color = linkColor;

		//positions local to the panel it is on (image panel coordinates), translated to fit
		//pixelDeviation box.
		Vector2 startPos = new Vector2((float) link.StartPos.X, (float) link.StartPos.Y);
		Vector2 endPos = new Vector2((float) link.EndPos.X, (float) link.EndPos.Y);

		//positions local to the Panel object in wallet.
		Vector2 localStartPos = new Vector3((startPos.x - col * Commons.panelResolutionX) / (pixelDeviation * 2) - 0.50f
			, -((startPos.y - row * Commons.panelResolutionY)  / (pixelDeviation * 2) - 0.50f)
			, 0.0f);

		Vector2 localEndPos = new Vector3((endPos.x - col * Commons.panelResolutionX) / (pixelDeviation * 2) - 0.50f
			, -((endPos.y - row * Commons.panelResolutionY) / (pixelDeviation * 2) - 0.50f)
			, 0.0f);
		
		float lineLength = section.transform
			.TransformDirection(localStartPos - localEndPos).magnitude;
		
		cylinderClone.transform.SetParent(section.transform);
		cylinderClone.transform.localScale = new Vector3(0.005f
			,  lineLength / 2
			, 0.005f);
		cylinderClone.transform.localPosition = (localStartPos + localEndPos) / 2;

		// Calculation of angle to rotate the Cylinder gameobject by. This is done using
		// pythagoras theorem, by taking change in x between end points over line length / 2.

		float deltaXOverH = (Math.Abs(localEndPos.x - cylinderClone.transform.localPosition.x)) / (lineLength / 2);

		// Due to floating point imprecision and rounding, we might end up with deltaXOverH >= 1.0f
		// or <= -1.0f. We have to bound deltaXOverH to -1 < deltaXOverH < 1 for it to make sense
		// mathematically.
		if (deltaXOverH > 1.0f) {
			deltaXOverH = 0.99999f;
		} else if (deltaXOverH < -1.0f) {
			deltaXOverH = -0.99999f;
		}
		float angle = Mathf.Rad2Deg * Mathf.Acos(deltaXOverH);
		
		int quadrantFlags = 0;
		// checks relative position of endpos.
		quadrantFlags += cylinderClone.transform.localPosition.x < localEndPos.x ? 1 << 0 : 0;
		quadrantFlags += cylinderClone.transform.localPosition.y < localEndPos.y ? 1 << 1 : 0;

		switch (quadrantFlags) {
			//endpos is left of and below
			case 0:
				cylinderClone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f + angle);
				break;
			//endpos is left of and above
			case 1:
				cylinderClone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f - angle);
				break;
			//endpos is right of and below
			case 2:
				cylinderClone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -(90.0f + angle));
				break;
			//endpos is right of and above
			case 3:
				cylinderClone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, - (90.0f - angle));
				break;
			default:
				Debug.Log("ran into default case?");
				break;
		}
	}

	private class DataRect {
		public int panelNumber;
		public Vector2 topLeft;
		public Vector2 bottomRight;

		public DataRect(int panelNumber, Vector2 topLeft, Vector2 bottomRight) {
			this.panelNumber = panelNumber;
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
	}

	private UnityWebRequest SendRequest(DataRect rect) {
		//panel number is base 1
		int row = (rect.panelNumber - 1) / 16;
		int col = (rect.panelNumber - 1) % 16;

		//Gets node and link details.
		string dataURL = string.Format("{0}/pocket_json/?row={1}&col={2}&topleftx={3}&toplefty={4}&bottomrightx={5}&bottomrighty={6}"
			, new object[] {
				Commons.dataURL,
				row,
				col,
				rect.topLeft.x,
				rect.topLeft.y,
				rect.bottomRight.x,
				rect.bottomRight.y
			});

		Debug.Log(dataURL);

		UnityWebRequest req = UnityWebRequest.Get(dataURL);
		req.SendWebRequest();
		return req;
	}
}
