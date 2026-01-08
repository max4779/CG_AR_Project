using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARMultiTrackedImageController : MonoBehaviour
{
    public ARTrackedImageManager arTrackedImageManager;
    public CSVLoader csvLoader;

    private Dictionary<string, AnchorData> anchorDataDict;
    private Dictionary<string, GameObject> prefabMap;
    private Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();

    private bool ready = false;

    void Start()
    {
        // 1️⃣ Prefab 전부 로딩
        prefabMap = PrefabLoader.LoadAllPrefabs();

        // 2️⃣ CSV 로딩
        StartCoroutine(csvLoader.LoadAnchorDataFromStreamingAssets(OnCSVLoaded));
    }

    void OnCSVLoaded(Dictionary<string, AnchorData> data)
    {
        anchorDataDict = data;
        ready = true;

        Debug.Log("[AR] CSV loaded = " + data.Count);
    }

    void OnEnable()
    {
        arTrackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        arTrackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (!ready) return;

        foreach (var img in args.added)
            Handle(img);

        foreach (var img in args.updated)
            Handle(img);
    }

    void Handle(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking)
            return;

        string marker = img.referenceImage.name;

        if (!anchorDataDict.ContainsKey(marker))
            return;

        AnchorData data = anchorDataDict[marker];

        if (!prefabMap.ContainsKey(data.objectName))
        {
            Debug.LogError("[AR] Prefab not found: " + data.objectName);
            return;
        }

        if (!spawned.ContainsKey(marker))
        {
            GameObject obj = Instantiate(prefabMap[data.objectName]);
            obj.name = marker + "_Instance";
            obj.SetActive(false);
            spawned[marker] = obj;

            Debug.Log("[AR] Spawned " + data.objectName + " for " + marker);
        }

        GameObject active = spawned[marker];

        active.transform.SetPositionAndRotation(
            img.transform.position,
            img.transform.rotation
        );

        active.transform.Translate(data.position, Space.Self);
        active.transform.Rotate(data.rotation, Space.Self);
        active.transform.localScale = data.scale;

        active.SetActive(true);
    }
}
