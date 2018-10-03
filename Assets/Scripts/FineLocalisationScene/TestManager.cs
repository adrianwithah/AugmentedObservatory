using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.XR.WSA;

public class TestManager : NonPersistSingleton<TestManager> {

	int cellWidth = 30;
	int cellHeight = 30;
	int imageWidth = 1920;
	int imageHeight = 1080;

	int numCellsPerRow;
	int numCellsPerCol;

	Vector2[] actualCellCoordinates;
	Vector2[] coordinatesPressed;
	int stageCounter = 1;
	bool readyForInput = false;
	TextMesh logger;

	// Use this for initialization
	void Start () {
		logger = GameObject.Find("TextLogger").GetComponent<TextMesh>();
		logger.text = "Instruction: Please press Start button when ready.";

		numCellsPerRow = imageWidth / cellWidth;
		numCellsPerCol = imageHeight / cellHeight;

		int innerXOffset = numCellsPerRow / 6;
		int innerYOffset = numCellsPerCol / 6;

		int secondInnerXOffset = innerXOffset * 2;
		int secondInnerYOffset = innerYOffset * 2;

		actualCellCoordinates = new Vector2[] {
			new Vector2(0, 0),
			new Vector2(numCellsPerRow - 1, 0),
			new Vector2(0, numCellsPerCol - 1),
			new Vector2(numCellsPerRow - 1, numCellsPerCol -1),

			new Vector2(innerXOffset - 1, innerYOffset),
			new Vector2(numCellsPerRow - (innerXOffset + 1), innerYOffset),
			new Vector2(innerXOffset - 1, numCellsPerCol - (innerYOffset + 1)),
			new Vector2(numCellsPerRow - (innerXOffset + 1), numCellsPerCol - (innerYOffset + 1)),

			new Vector2(secondInnerXOffset - 1, secondInnerYOffset),
			new Vector2(numCellsPerRow - (secondInnerXOffset + 1), secondInnerYOffset),
			new Vector2(secondInnerXOffset - 1, numCellsPerCol - (secondInnerYOffset + 1)),
			new Vector2(numCellsPerRow - (secondInnerXOffset + 1), numCellsPerCol - (secondInnerYOffset + 1)),

			new Vector2(numCellsPerRow / 2 - 1, numCellsPerCol / 2 - 1)
		};

		coordinatesPressed = new Vector2[actualCellCoordinates.Length];
	}

	// In order not to affect the user's viewport, we will make the augmented panels/columns
	// invisible when the user starts the test.
	public void StartTest() {
		SpatialMappingManager spatialMappingManager = SpatialMappingManager.Instance;

		var anchors = FindObjectsOfType<WorldAnchor>();
		for (var i = 0; i < anchors.Length; i++) {
			// Don't remove SpatialMapping anchors if exists
			if (spatialMappingManager != null && anchors[i].gameObject.transform.parent.gameObject == spatialMappingManager.gameObject) {
				continue;
			}

			MeshRenderer meshRenderer;
			if ((meshRenderer = anchors[i].gameObject.GetComponent<MeshRenderer>()) != null) {
				meshRenderer.enabled = false;
			}
			foreach (Transform child in anchors[i].gameObject.transform) {
				if ((meshRenderer = child.gameObject.GetComponent<MeshRenderer>()) != null) {
					meshRenderer.enabled = false;
				}
			}
		}

		stageCounter = 1;
		StartNextStage(stageCounter);	
	}

	public void SubmitCoordinatesPressed(Vector3 coordinatesLocalToObject, GameObject go) {
		if (!readyForInput) {
			return;
		}

		readyForInput = false;
		coordinatesPressed[stageCounter - 1] = ConvertToCellCoordinates(coordinatesLocalToObject, go);
		stageCounter++;
		StartNextStage(stageCounter);
	}

	private void StartNextStage(int currStageCounter) {
		switch (currStageCounter) {
			case 1:
				logger.text = "Instruction: Please tap on the top left corner of the outermost rectangle.";
				readyForInput = true;
				break;
			case 2:
				logger.text = "Instruction: Please tap on the top right corner of the outermost rectangle.";
				readyForInput = true;
				break;
			case 3:
				logger.text = "Instruction: Please tap on the bottom left corner of the outermost rectangle.";
				readyForInput = true;
				break;
			case 4:
				logger.text = "Instruction: Please tap on the bottom right corner of the outermost rectangle.";
				readyForInput = true;
				break;
			case 5:
				logger.text = "Instruction: Please tap on the top left corner of the second outermost rectangle.";
				readyForInput = true;
				break;
			case 6:
				logger.text = "Instruction: Please tap on the top right corner of the second outermost rectangle.";
				readyForInput = true;
				break;
			case 7:
				logger.text = "Instruction: Please tap on the bottom left corner of the second outermost rectangle.";
				readyForInput = true;
				break;
			case 8:
				logger.text = "Instruction: Please tap on the bottom right corner of the second outermost rectangle.";
				readyForInput = true;
				break;
			case 9:
				logger.text = "Instruction: Please tap on the top left corner of the innermost rectangle.";
				readyForInput = true;
				break;
			case 10:
				logger.text = "Instruction: Please tap on the top right corner of the innermost rectangle.";
				readyForInput = true;
				break;
			case 11:
				logger.text = "Instruction: Please tap on the bottom left corner of the innermost rectangle.";
				readyForInput = true;
				break;
			case 12:
				logger.text = "Instruction: Please tap on the bottom right corner of the innermost rectangle.";
				readyForInput = true;
				break;
			case 13:
				logger.text = "Instruction: Please tap the center black square.";
				readyForInput = true;
				break;
			default:
				logger.text = "Thank you. You have finished the test.";
				EndTest();
				break;
		}
	}

	private void EndTest() {
		SpatialMappingManager spatialMappingManager = SpatialMappingManager.Instance;

		var anchors = FindObjectsOfType<WorldAnchor>();
		for (var i = 0; i < anchors.Length; i++) {
			// Don't remove SpatialMapping anchors if exists
			if (spatialMappingManager != null && anchors[i].gameObject.transform.parent.gameObject == spatialMappingManager.gameObject) {
				continue;
			}

			MeshRenderer meshRenderer;
			if ((meshRenderer = anchors[i].gameObject.GetComponent<MeshRenderer>()) != null) {
				meshRenderer.enabled = true;
			}
			foreach (Transform child in anchors[i].gameObject.transform) {
				if ((meshRenderer = child.gameObject.GetComponent<MeshRenderer>()) != null) {
					meshRenderer.enabled = true;
				}
			}
		}

		GenerateResults();
	}

	private void GenerateResults() {
		for (int i = 0; i < actualCellCoordinates.Length; i++) {
			Debug.LogFormat("{0}: Actual Cell: {1}, Cell Pressed: {2}, Num Panels Away: {3}"
				, new object[] {
					i,
					actualCellCoordinates[i],
					coordinatesPressed[i],
					GetNumCellsAway(actualCellCoordinates[i], coordinatesPressed[i])	
				});
		}
	}

	private int GetNumCellsAway(Vector2 actualCoord, Vector2 pressedCoord) {
		return Mathf.FloorToInt(Mathf.Max(Mathf.Abs(actualCoord.x - pressedCoord.x), Mathf.Abs(actualCoord.y - pressedCoord.y)));
	}

	private Vector2 ConvertToCellCoordinates(Vector3 coordinatesLocalToObject, GameObject go) {
		
		// Coordinates are local to object, with (0, 0) at center of object.

		BoxCollider collider = go.GetComponent<BoxCollider>();

		if (collider == null) {
			Debug.LogError("Unable to obtain collider of gameobject: " + go);
			return new Vector2(-1, -1);
		}

		float width = collider.size.x;
		float height = collider.size.y;

		// We treat (0, 0) as top left of object and translate the point into the new
		// coordinate system.
		coordinatesLocalToObject = (coordinatesLocalToObject + (new Vector3((width/ 2), -(height / 2), 0)));
		coordinatesLocalToObject.y *= -1;
		
		Debug.Log("Re-centered coordinatesLocalToObject: " + coordinatesLocalToObject);

		//Change to imageWidth and imageHeight coordinates system.
		Vector2 imageCoord = new Vector2(0, 0);
		imageCoord.x = imageWidth / width * coordinatesLocalToObject.x;
		imageCoord.y = imageHeight / height * coordinatesLocalToObject.y;

		// imageCoord is the metric used to determine pixel deviations between points.
		Debug.Log("Coords after converting to image dimensions: " + imageCoord);

		Vector2 cellCoord = new Vector2(imageCoord.x / cellWidth, imageCoord.y / cellHeight);
		Debug.Log("Cell coordinate: " + cellCoord);
		return cellCoord;
	}
}	
