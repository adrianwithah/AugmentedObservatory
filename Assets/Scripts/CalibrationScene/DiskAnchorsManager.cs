using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Dialog;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Sharing;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Storage.Streams;
#endif

public class DiskAnchorsManager : Singleton<DiskAnchorsManager> {

	public Dialog dialogPrefab;
	private Toolbar toolBar;
	public GameObject columnObjectToAnchor;
	public GameObject panelObjectToAnchor;
	private List<byte> cachedExportData;
	
	private readonly string panelDataFileName = "panelAnchor.dat";
	private readonly string columnDataFileName = "columnAnchor.dat";

#if WINDOWS_UWP

	private StorageFolder storageFolder = KnownFolders.PicturesLibrary;
	
#endif

	// Use this for initialization
	void Start () {
		toolBar = GameObject.Find("Toolbar").GetComponent<Toolbar>();
		cachedExportData = new List<byte>(0);
	}

	//Loading and Saving to and from DISK.
	public void SaveAllPanelAnchorsToDisk()
    {
		OnSaveAnchorsToDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding all World Anchors attached to FixedPanels in scene...");

		WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
	
		WorldAnchor[] anchors = FindObjectsOfType<WorldAnchor>();
		foreach (WorldAnchor anchor in anchors) {
			//Since we only anchor FixedPanel and FixedColumn, we can start by checking if it has FixedPanel.
			FixedPanel fixedPanel = anchor.gameObject.GetComponent<FixedPanel>();
			if (fixedPanel == null) {
				continue;
			}

			batch.AddWorldAnchor(fixedPanel.ImageTarget.name, anchor);
		}

		ProgressIndicator.Instance.SetProgress(0.10f);
		ProgressIndicator.Instance.SetMessage("Serializing World Anchors to bytes...");

		WorldAnchorTransferBatch.ExportAsync(batch, OnExportDataAvailable, OnExportCompletePanel);
    }

    public void SaveAllColumnAnchorsToDisk()
    {
		OnSaveAnchorsToDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding all World Anchors attached to FixedColumns in scene...");

        WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
	
		WorldAnchor[] anchors = FindObjectsOfType<WorldAnchor>();
		foreach (WorldAnchor anchor in anchors) {
			//If its a FixedColumn, then it has 4 children.
			if (anchor.gameObject.transform.childCount != 4) {
				Debug.LogFormat("{0} does not have 4 children, cannot be FixedColumn. Skipping anchor!", anchor.name);
				continue;
			}

			//Check that first children has FixedColumnPanel.
			FixedColumnPanel columnPanel 
				= anchor.gameObject.transform.GetChild(0).gameObject.GetComponent<FixedColumnPanel>();
			if (columnPanel == null) {
				Debug.LogFormat("First child of object: {0} does not have FixedColumnPanel component. Skipping anchor!", anchor.name);
				continue;
			}

			batch.AddWorldAnchor(columnPanel.ImageTarget.name, anchor);
		}

		ProgressIndicator.Instance.SetProgress(0.10f);
		ProgressIndicator.Instance.SetMessage("Serializing World Anchors to bytes...");

		WorldAnchorTransferBatch.ExportAsync(batch, OnExportDataAvailable, OnExportCompleteColumn);
    }

	private void OnExportDataAvailable(byte[] data) {
		ProgressIndicator.Instance.SetMessage(
			string.Format("Bytes received. Caching {0} bytes...", data.Length));

		cachedExportData.AddRange(data);
	}

	private void OnExportCompletePanel(SerializationCompletionReason completionReason) {
		if (completionReason != SerializationCompletionReason.Succeeded) {
			Debug.Log("An error has occurred while exporting WorldAnchors..");

			Dialog.Open(dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Error serializing World Anchors. Please try again.");

			OnSaveAnchorsToDiskCompleted();
		} else {
			ProgressIndicator.Instance.SetMessage("Serialization successful. Now writing cached data to file.");
			ProgressIndicator.Instance.SetProgress(0.80f);

			SaveCachedDataToFile(panelDataFileName);
		}
	}

	private void OnExportCompleteColumn(SerializationCompletionReason completionReason) {
		if (completionReason != SerializationCompletionReason.Succeeded) {
			ProgressIndicator.Instance.SetMessage("Error serializing World Anchors, aborting save..");

			Debug.Log("An error has occurred while exporting WorldAnchors..");

			Dialog.Open(dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Error serializing World Anchors. Please try again.");

			OnSaveAnchorsToDiskCompleted();
		} else {
			ProgressIndicator.Instance.SetMessage("Serialization successful. Now writing cached data to file.");
			ProgressIndicator.Instance.SetProgress(0.80f);

			SaveCachedDataToFile(columnDataFileName);
		}
	}

	private async void SaveCachedDataToFile(string fileName) {
		#if WINDOWS_UWP
			Debug.Log("Creating anchor data file.");
			StorageFile dataFile 
				= await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

			Debug.Log("Writing cached export data to data file.");
			FileIO.WriteBytesAsync(dataFile, cachedExportData.ToArray());
			Debug.LogFormat("Number of bytes saved: {0}", cachedExportData.Count);

			ProgressIndicator.Instance.SetProgress(1.0f);

		#else
			Dialog.Open(dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Unable to save cached data to file as this is not a UWP system.");

			Debug.Log("Unable to save cached data to file as this is not a UWP system.");
		#endif

		OnSaveAnchorsToDiskCompleted();
	}

	private void OnSaveAnchorsToDiskStarted()
    {
		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.None,
                            ProgressStyleEnum.ProgressBar,
                            ProgressMessageStyleEnum.Visible,
                            "Initiating save anchors to disk.");

        toolBar.DisableAllButtons();
		toolBar.GetComponent<Billboard>().enabled = false;
		toolBar.GetComponent<Tagalong>().enabled = false;
    }

	private void OnSaveAnchorsToDiskCompleted() {
		ProgressIndicator.Instance.Close();
		cachedExportData.Clear();
		toolBar.EnableAllButtons();
		toolBar.GetComponent<Billboard>().enabled = true;
		toolBar.GetComponent<Tagalong>().enabled = true;
	}

	public async void LoadAllPanelAnchorsFromDisk()
    {
		OnLoadAnchorsFromDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding data file..");

		#if WINDOWS_UWP
			IStorageItem item = await storageFolder.TryGetItemAsync(panelDataFileName);
			if (item == null) {
				Debug.LogFormat("Unable to locate data file at: {0}!", storageFolder.Path);

				Dialog.Open(dialogPrefab.gameObject
					, DialogButtonType.OK
					, "Error"
					, string.Format("Unable to locate data file at: {0}!", storageFolder.Path));
				
				return;
			}

			StorageFile dataFile = (StorageFile) item;

			IBuffer buffer = await FileIO.ReadBufferAsync(dataFile);
			byte[] dataByteArray = new byte[buffer.Length];

			using (DataReader dataReader = DataReader.FromBuffer(buffer)) {
				dataReader.ReadBytes(dataByteArray);	
			}
			
			ProgressIndicator.Instance.SetMessage("Found file. Now importing World Anchors..");
			ProgressIndicator.Instance.SetProgress(0.10f);

			Debug.LogFormat("Number of bytes read: {0}", dataByteArray.Length);
			WorldAnchorTransferBatch.ImportAsync(dataByteArray, OnImportColumnDataComplete);
			
		#else
			Dialog.Open(dialogPrefab.gameObject
				, DialogButtonType.OK
				, "Error"
				, "Unable to load anchor data from file as this is not a UWP system.");

			Debug.Log("Unable to load anchor data from file as this is not a UWP system.");
			OnLoadAnchorsFromDiskCompleted();
		#endif
    }

	private void OnImportPanelDataComplete(SerializationCompletionReason completionReason
		, WorldAnchorTransferBatch batch) {
		
		if (completionReason != SerializationCompletionReason.Succeeded) {
			Debug.Log("Import failed! Retrying load...");
			LoadAllPanelAnchorsFromDisk();
			return;
		}

		ProgressIndicator.Instance.SetMessage("Instantiating panels with World Anchors..");
		ProgressIndicator.Instance.SetProgress(0.60f);

		InstantiateFixedPanels(batch);
	}

    private void InstantiateFixedPanels(WorldAnchorTransferBatch batch)
    {
        string[] ids = batch.GetAllIds();
		int numAnchorsSkipped = 0;

		for (int i = 0; i < ids.Length; i++) {
			ProgressIndicator.Instance.SetProgress(0.60f + i / ids.Length * 0.35f);

			if (!IsValidPanelAnchorId(ids[i])) {
				Debug.LogFormat("Found anchor that does not correspond to a panel image target. Skipping load for \"{0}\".", ids[i]);
				numAnchorsSkipped++;
				continue;
			}
		
			GameObject panelToAnchor = InstantiatePanelToAnchor(ids[i]);

			if (panelToAnchor == null) {
				numAnchorsSkipped++;
				continue;
			}

			WorldAnchor anchor = batch.LockObject(ids[i], panelToAnchor);
			
			if (anchor == null) {
				numAnchorsSkipped++;
				Debug.LogFormat("WorldAnchor with id: {0} could not be locked!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Panel with id: {0}", ids[i]);

				//Get WorldAnchorManager to take over tracking of anchor.
				WorldAnchorManager.Instance.AttachAnchor(panelToAnchor, ids[i]);
			}	
		}

	
		Dialog.Open(dialogPrefab.gameObject
			, DialogButtonType.OK
			, "Error"
			, string.Format("{0} WorldAnchors loaded and {1} WorldAnchors skipped!"
				, ids.Length - numAnchorsSkipped
				, numAnchorsSkipped));
    }

	private GameObject InstantiatePanelToAnchor(string anchorId) {
		GameObject imageTarget
			= TargetsManager.Instance.GetImageTarget(TargetsManager.GetPanelNumberFromPanelName(anchorId));
		if (imageTarget == null) {
			Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped loading anchor.", anchorId);
			return null;
		}

		GameObject clone = GameObject.Instantiate(panelObjectToAnchor
			, panelObjectToAnchor.transform.position
			, panelObjectToAnchor.transform.rotation);
		clone.transform.localScale = new Vector3 (1.1f, 0.6f, 0.2f);

		clone.GetComponent<FixedPanel>().RegisterImageTarget(imageTarget);

		return clone;
	}

	public static bool IsValidPanelAnchorId(string id) {
		return id.StartsWith(TargetsManager.panelImageTargetPrefix);
	}

    public async void LoadAllColumnAnchorsFromDisk()
    {
		OnLoadAnchorsFromDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding data file..");

        #if WINDOWS_UWP
			IStorageItem item = await storageFolder.TryGetItemAsync(columnDataFileName);
			if (item == null) {
				Debug.LogFormat("Unable to locate data file at: {0}!", storageFolder.Path);

				Dialog.Open(dialogPrefab.gameObject
					, DialogButtonType.OK
					, "Error"
					, string.Format("Unable to locate data file at: {0}!", storageFolder.Path));

				return;
			} else {
				Debug.Log("Found column anchor data file!");
			}

			StorageFile dataFile = (StorageFile) item;

			IBuffer buffer = await FileIO.ReadBufferAsync(dataFile);
			byte[] dataByteArray = new byte[buffer.Length];

			using (DataReader dataReader = DataReader.FromBuffer(buffer)) {
				dataReader.ReadBytes(dataByteArray);	
			}

			ProgressIndicator.Instance.SetMessage("Found file. Now importing World Anchors..");
			ProgressIndicator.Instance.SetProgress(0.10f);

			Debug.LogFormat("Number of bytes read: {0}", dataByteArray.Length);
			WorldAnchorTransferBatch.ImportAsync(dataByteArray, OnImportColumnDataComplete);
			
		#else
			Debug.Log("Unable to load anchor data from file as this is not a UWP system.");

			Dialog.Open(dialogPrefab.gameObject
					, DialogButtonType.OK
					, "Error"
					, "Unable to load anchor data from file as this is not a UWP system.");

			OnLoadAnchorsFromDiskCompleted();
		#endif
    }

	private void InstantiateFixedColumns(WorldAnchorTransferBatch batch)
    {
        string[] ids = batch.GetAllIds();

		int numAnchorsSkipped = 0;

		Debug.LogFormat("Found {0} Anchors from the data file!", ids.Length);

		for (int i = 0; i < ids.Length; i++) {

			ProgressIndicator.Instance.SetProgress(0.60f + i / ids.Length * 0.35f);

			if (!IsValidColumnAnchorId(ids[i])) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				numAnchorsSkipped++;
				continue;
			}
		
			GameObject columnToAnchor = InstantiateColumnToAnchor(ids[i]);
			if (columnToAnchor == null) {
				numAnchorsSkipped++;
				continue;
			}

			WorldAnchor anchor = batch.LockObject(ids[i], columnToAnchor);
			if (anchor == null) {
				numAnchorsSkipped++;
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
				continue;
			}

			Debug.LogFormat("Loaded WorldAnchor for Panel with id: {0}", ids[i]);

			//Get WorldAnchorManager to take over tracking of anchor.
			WorldAnchorManager.Instance.AttachAnchor(columnToAnchor, ids[i]);
		}

		Dialog.Open(dialogPrefab.gameObject
			, DialogButtonType.OK
			, "Success"
			, string.Format("{0} WorldAnchors loaded and {1} WorldAnchors skipped!"
				, ids.Length - numAnchorsSkipped
				, numAnchorsSkipped));

		OnLoadAnchorsFromDiskCompleted();
    }

	private void OnLoadAnchorsFromDiskStarted() {
		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.None,
                            ProgressStyleEnum.ProgressBar,
                            ProgressMessageStyleEnum.Visible,
                            "Initiating load anchors from disk.");

		toolBar.DisableAllButtons();
		toolBar.GetComponent<Billboard>().enabled = false;
		toolBar.GetComponent<Tagalong>().enabled = false;
	}

	private void OnLoadAnchorsFromDiskCompleted() {
		ProgressIndicator.Instance.Close();
		toolBar.EnableAllButtons();
		toolBar.GetComponent<Billboard>().enabled = true;
		toolBar.GetComponent<Tagalong>().enabled = true;
	}

	private GameObject InstantiateColumnToAnchor(string anchorId) {
		GameObject imageTarget 
			= TargetsManager.Instance.GetColumnImageTarget(TargetsManager.GetColumnNumberFromColumnName(anchorId));

		if (imageTarget == null) {
			Debug.LogFormat("Unable to find corresponding imageTarget: \"{0}\" in scene. Skipped loading anchor.", anchorId);
			return null;
		}

		GameObject clone = GameObject.Instantiate(columnObjectToAnchor);
		foreach (Transform childTransform in clone.transform) {
			Debug.LogFormat("Adding fixed column panel for child: {0}", childTransform.gameObject.name);
			childTransform.gameObject.GetComponent<FixedColumnPanel>().RegisterImageTarget(imageTarget);
		}
		clone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

		return clone;
	}

    private void OnImportColumnDataComplete(SerializationCompletionReason completionReason
		, WorldAnchorTransferBatch batch) {
		
		if (completionReason != SerializationCompletionReason.Succeeded) {
			Debug.Log("Import failed! Retrying load...");
			LoadAllColumnAnchorsFromDisk();
			return;
		}

		ProgressIndicator.Instance.SetMessage("Instantiating panels with World Anchors..");
		ProgressIndicator.Instance.SetProgress(0.60f);

		Debug.Log("Import succeeded! Instantiating fixed columns now..");

		InstantiateFixedColumns(batch);
	}

	public static bool IsValidColumnAnchorId(string id) {
		return id.StartsWith(TargetsManager.columnImageTargetPrefix);
	}
}
