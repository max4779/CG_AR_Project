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
        string marker = img.referenceImage.name;

        // 아직 CSV 안 로드됐거나, 해당 마커 데이터 없음
        if (!anchorDataDict.ContainsKey(marker))
            return;

        // 아직 생성 안 된 상태면 생성만 해둠
        if (!spawned.ContainsKey(marker))
        {
            AnchorData data = anchorDataDict[marker];

            if (!prefabMap.ContainsKey(data.objectName))
            {
                Debug.LogError("[AR] Prefab not found: " + data.objectName);
                return;
            }

            GameObject obj = Instantiate(
                prefabMap[data.objectName],
                img.transform   // 마커를 부모로
            );

            obj.name = marker + "_Instance";
            obj.transform.localPosition = data.position;
            obj.transform.localEulerAngles = data.rotation;
            obj.transform.localScale = data.scale;

            obj.SetActive(false);   // ⭐ 처음엔 비활성화
            spawned[marker] = obj;
        }

        // 🔴 핵심 분기
        if (img.trackingState == TrackingState.Tracking)
        {
            spawned[marker].SetActive(true);
        }
        else
        {
            // ⭐ 마커가 안 보이면 반드시 끈다
            spawned[marker].SetActive(false);
        }
    }


}
