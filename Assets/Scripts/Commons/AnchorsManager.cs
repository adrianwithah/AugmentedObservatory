using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Dialog;
using HoloToolkit.UX.Progress;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA.Sharing;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Storage.Streams;
#endif

// Wrapper based on WorldAnchorStore. Extends the WorldAnchorManager functionality
// to include loading and saving to and from disk utilising WorldAnchorBatches.
// Will hand off tracking to WorldAnchorManager after loading anchors from disk.

// Note: Anchors are saved using the same name as their corresponding Image Target name.
// This helps facilitate reuse of Anchors across App Instances.
public class AnchorsManager : Singleton<AnchorsManager> {

	public Action DiskSaveCompletedAction;
	public Action<Dictionary<String, GameObject>> DiskLoadCompletedAction;
	// An ordered list to cache the streams of serialized data in the order of receival.
	private List<byte> cachedExportData = new List<byte>(0);
	private readonly string panelDataFileName = "panelAnchor.dat";
	private readonly string columnDataFileName = "columnAnchor.dat";

	// Scale of the augmented panels/columns that match the physical size. This is
	// dependent on the scale of the QR code markers in Unity Coordinates, which is
	// determined by the Unity application at runtime. Since size of QR code markers are constant,
	// these values should remain accurate.
	public static readonly Vector3 panelObjectLossyScale 
		= new Vector3(1.020717f, 0.5741533f, 0.159487f);
	public static readonly Vector3 columnObjectLossyScale 
		= new Vector3(0.159487f, 0.159487f, 0.159487f);

#if WINDOWS_UWP

	private StorageFolder storageFolder = KnownFolders.PicturesLibrary;
	
#endif

	//Loading and Saving to and from DISK.
	public void SaveAllPanelAnchorsToDisk(WorldAnchor[] panelAnchors) {
		OnSaveAnchorsToDiskStarted();

		WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
	
		foreach (WorldAnchor anchor in panelAnchors) {
			batch.AddWorldAnchor(anchor.gameObject.name, anchor);
		}

		ProgressIndicator.Instance.SetProgress(0.10f);
		ProgressIndicator.Instance.SetMessage("Serializing World Anchors to bytes...");

		WorldAnchorTransferBatch.ExportAsync(batch, OnExportDataAvailable, OnExportCompletePanel);
    }

    public void SaveAllColumnAnchorsToDisk(WorldAnchor[] columnAnchors) {
		OnSaveAnchorsToDiskStarted();

        WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
	
		foreach (WorldAnchor anchor in columnAnchors) {
			batch.AddWorldAnchor(anchor.gameObject.name, anchor);
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

			Dialog.Open(PrefabsManager.Instance.dialogPrefab
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

			Dialog.Open(PrefabsManager.Instance.dialogPrefab
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
			Dialog.Open(PrefabsManager.Instance.dialogPrefab
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
    }

	private void OnSaveAnchorsToDiskCompleted() {
		ProgressIndicator.Instance.Close();
		cachedExportData.Clear();

		DiskSaveCompletedAction.Invoke();
	}

	public async void LoadAllPanelAnchorsFromDisk(GameObject panelObjectToAnchor)
    {
		OnLoadAnchorsFromDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding data file..");

		#if WINDOWS_UWP
			IStorageItem item = await storageFolder.TryGetItemAsync(panelDataFileName);
			if (item == null) {
				Debug.LogFormat("Unable to locate data file at: {0}!", storageFolder.Path);

				Dialog.Open(PrefabsManager.Instance.dialogPrefab
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
			WorldAnchorTransferBatch.ImportAsync(dataByteArray
				, (completionReason, batch) 
					=> OnImportPanelDataComplete(panelObjectToAnchor, completionReason, batch));
			
		#else
			Dialog.Open(PrefabsManager.Instance.dialogPrefab
				, DialogButtonType.OK
				, "Error"
				, "Unable to load anchor data from file as this is not a UWP system.");

			Debug.Log("Unable to load anchor data from file as this is not a UWP system.");
			OnLoadAnchorsFromDiskCompleted(null);
		#endif
    }

	private void OnImportPanelDataComplete(GameObject panelObjectToAnchor
		, SerializationCompletionReason completionReason
		, WorldAnchorTransferBatch batch) {
		
		if (completionReason != SerializationCompletionReason.Succeeded) {
			Debug.Log("Import failed! Retrying load...");
			LoadAllPanelAnchorsFromDisk(panelObjectToAnchor);
			return;
		}

		ProgressIndicator.Instance.SetMessage("Instantiating panels with World Anchors..");
		ProgressIndicator.Instance.SetProgress(0.60f);

		InstantiateFixedPanels(batch, panelObjectToAnchor);
	}

	// On successful load of WorldAnchors from disk, we iterate through the anchors 
	// instantiate panels using the FixedPanel prefab for each anchor. These objects
	// are added to a dictionary which is then passed to the OnLoadCompleted method.
    private void InstantiateFixedPanels(WorldAnchorTransferBatch batch
		, GameObject panelObjectToAnchor) {

		Dictionary<String, GameObject> anchorIdToObject = new Dictionary<string, GameObject>();
        string[] ids = batch.GetAllIds();
		int numAnchorsSkipped = 0;

		for (int i = 0; i < ids.Length; i++) {
			ProgressIndicator.Instance.SetProgress(0.60f + i / ids.Length * 0.35f);

			if (!IsValidPanelAnchorId(ids[i])) {
				Debug.LogFormat("Found anchor that does not correspond to a panel image target. Skipping load for \"{0}\".", ids[i]);
				numAnchorsSkipped++;
				continue;
			}
		
			GameObject panelToAnchor = GameObject.Instantiate(panelObjectToAnchor
				, panelObjectToAnchor.transform.position
				, panelObjectToAnchor.transform.rotation);
			panelToAnchor.transform.localScale = panelObjectLossyScale;

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

				anchorIdToObject.Add(ids[i], panelToAnchor);
			}	
		}

	
		Dialog.Open(PrefabsManager.Instance.dialogPrefab
			, DialogButtonType.OK
			, "Success"
			, string.Format("{0} WorldAnchors loaded and {1} WorldAnchors skipped!"
				, ids.Length - numAnchorsSkipped
				, numAnchorsSkipped));
		
		OnLoadAnchorsFromDiskCompleted(anchorIdToObject);
    }

	// Anchor id and Image Target naming conventions are the same.
	public static bool IsValidPanelAnchorId(string id) {
		return id.StartsWith(TargetsManager.panelImageTargetPrefix);
	}

    public async void LoadAllColumnAnchorsFromDisk(GameObject columnObjectToAnchor)
    {
		OnLoadAnchorsFromDiskStarted();

		ProgressIndicator.Instance.SetMessage("Finding data file..");

        #if WINDOWS_UWP
			IStorageItem item = await storageFolder.TryGetItemAsync(columnDataFileName);
			if (item == null) {
				Debug.LogFormat("Unable to locate data file at: {0}!", storageFolder.Path);

				Dialog.Open(PrefabsManager.Instance.dialogPrefab
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
			WorldAnchorTransferBatch.ImportAsync(dataByteArray
				, (completionReason, batch) =>
					OnImportColumnDataComplete(completionReason, batch, columnObjectToAnchor));
			
		#else
			Debug.Log("Unable to load anchor data from file as this is not a UWP system.");

			Dialog.Open(PrefabsManager.Instance.dialogPrefab
					, DialogButtonType.OK
					, "Error"
					, "Unable to load anchor data from file as this is not a UWP system.");

			OnLoadAnchorsFromDiskCompleted(new Dictionary<String, GameObject>());
		#endif
    }

	// On successful load of WorldAnchors from disk, we iterate through the anchors 
	// instantiate columns using the FixedColumn prefab for each anchor. These objects
	// are added to a dictionary which is then passed to the OnLoadCompleted method.
	private void InstantiateFixedColumns(WorldAnchorTransferBatch batch
		, GameObject columnObjectToAnchor)
    {
        string[] ids = batch.GetAllIds();

		Dictionary<String, GameObject> anchorIdToObject = new Dictionary<String, GameObject>();

		int numAnchorsSkipped = 0;

		Debug.LogFormat("Found {0} Anchors from the data file!", ids.Length);

		for (int i = 0; i < ids.Length; i++) {

			ProgressIndicator.Instance.SetProgress(0.60f + i / ids.Length * 0.35f);

			if (!IsValidColumnAnchorId(ids[i])) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				numAnchorsSkipped++;
				continue;
			}

			GameObject columnToAnchor = GameObject.Instantiate(columnObjectToAnchor);
			columnToAnchor.transform.localScale = columnObjectLossyScale;

			WorldAnchor anchor = batch.LockObject(ids[i], columnToAnchor);
			if (anchor == null) {
				numAnchorsSkipped++;
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
				continue;
			}

			Debug.LogFormat("Loaded WorldAnchor for Column with id: {0}", ids[i]);

			//Get WorldAnchorManager to take over tracking of anchor.
			WorldAnchorManager.Instance.AttachAnchor(columnToAnchor, ids[i]);

			anchorIdToObject.Add(ids[i], columnToAnchor);
		}

		Dialog.Open(PrefabsManager.Instance.dialogPrefab
			, DialogButtonType.OK
			, "Success"
			, string.Format("{0} WorldAnchors loaded and {1} WorldAnchors skipped!"
				, ids.Length - numAnchorsSkipped
				, numAnchorsSkipped));

		OnLoadAnchorsFromDiskCompleted(anchorIdToObject);
    }

	private void OnLoadAnchorsFromDiskStarted() {
		ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.None,
                            ProgressStyleEnum.ProgressBar,
                            ProgressMessageStyleEnum.Visible,
                            "Initiating load anchors from disk.");
	}

	private void OnLoadAnchorsFromDiskCompleted(Dictionary<String, GameObject> anchorIdToObject) {
		ProgressIndicator.Instance.Close();
		
		DiskLoadCompletedAction.Invoke(anchorIdToObject);
	}

    private void OnImportColumnDataComplete(SerializationCompletionReason completionReason
		, WorldAnchorTransferBatch batch
		, GameObject columnObjectToAnchor) {
		
		if (completionReason != SerializationCompletionReason.Succeeded) {
			Debug.Log("Import failed! Retrying load...");
			LoadAllColumnAnchorsFromDisk(columnObjectToAnchor);
			return;
		}

		ProgressIndicator.Instance.SetMessage("Instantiating panels with World Anchors..");
		ProgressIndicator.Instance.SetProgress(0.60f);

		Debug.Log("Import succeeded! Instantiating fixed columns now..");

		InstantiateFixedColumns(batch, columnObjectToAnchor);
	}

	public static bool IsValidColumnAnchorId(string id) {
		return id.StartsWith(TargetsManager.columnImageTargetPrefix);
	}

	public Dictionary<String,GameObject> LoadAllColumnAnchorsFromStore(
		GameObject columnObjectToAnchor) {

		Debug.Log("Trying to load all column anchors from the store..");
		Dictionary<String, GameObject> anchorIdToObject = new Dictionary<String, GameObject>();

		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return anchorIdToObject;
		}

		
		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {
			
			if (!IsValidColumnAnchorId(ids[i])) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}

			GameObject clone = GameObject.Instantiate(columnObjectToAnchor);
			clone.transform.localScale = columnObjectLossyScale;

			if (store.Load(ids[i], clone) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Column with id: {0}.", ids[i]);
				anchorIdToObject.Add(ids[i], clone);
			}
		}

		return anchorIdToObject;
	}

	public Dictionary<String, GameObject> LoadAllPanelAnchorsFromStore(
			GameObject panelObjectToAnchor) {

		Debug.Log("Trying to load all panel anchors from the store..");
		Dictionary<String, GameObject> anchorIdToObject = new Dictionary<String, GameObject>();

		WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;

		if (store == null) {
			Debug.Log("Could not retrieve anchor store... WorldAnchorManager might not have been initialised.");
			return anchorIdToObject;
		}

		string[] ids = store.GetAllIds();
		for (int i = 0; i < ids.Length; i++) {

			if (!ids[i].StartsWith(TargetsManager.panelImageTargetPrefix)) {
				Debug.LogFormat("Found anchor that does not correspond to a column image target. Skipping load for \"{0}\".", ids[i]);
				continue;
			}

			GameObject anchoredObject = GameObject.Instantiate(panelObjectToAnchor);
			anchoredObject.transform.localScale = panelObjectLossyScale;

			if (store.Load(ids[i], anchoredObject) == null) {
				Debug.LogFormat("WorldAnchor with id: {0} could not be loaded!", ids[i]);
			} else {
				Debug.LogFormat("Loaded WorldAnchor for Panel with id: {0}", ids[i]);
				anchorIdToObject.Add(ids[i], anchoredObject);
			}
		}

		return anchorIdToObject;
	}
}