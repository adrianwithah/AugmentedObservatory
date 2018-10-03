using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

// Used to store prefabs that are used across multiple scenes, in response to
// the need to drag and drop prefabs onto multiple different objects.
public class PrefabsManager : Singleton<PrefabsManager> {

	public GameObject dialogPrefab;
	public GameObject fixedColumn;
	public GameObject fixedPanel;
	public GameObject appColumn;
	public GameObject appPanel;
}
