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
    private Dictionary<string, GameObject> spawned = new();

    private bool ready = false;

    void Start()
    {
        prefabMap = PrefabLoader.LoadAllPrefabs();
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

        // ⭐ 핵심: 현재 프레임에서 Tracking 중인 마커만 본다
        foreach (var img in arTrackedImageManager.trackables)
        {
            TrySpawnFromCSV(img);
        }
    }

    void TrySpawnFromCSV(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking)
            return;

        string marker = img.referenceImage.name;

        // CSV에 없는 마커는 무시
        if (!anchorDataDict.ContainsKey(marker))
            return;

        // 이미 생성됐으면 끝
        if (spawned.ContainsKey(marker))
            return;

        AnchorData data = anchorDataDict[marker];

        if (!prefabMap.ContainsKey(data.objectName))
        {
            Debug.LogError("[AR] Prefab not found: " + data.objectName);
            return;
        }

        GameObject obj = Instantiate(prefabMap[data.objectName], img.transform);
        obj.name = $"{marker}_{data.objectName}";

        // ⭐ CSV 값 그대로 적용
        obj.transform.localPosition = data.position;
        obj.transform.localEulerAngles = data.rotation;
        obj.transform.localScale = data.scale;

        spawned[marker] = obj;

        Debug.Log($"[AR] Spawned from CSV → {marker}");
    }
}
