using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.IO;

public class AnchorDataLoader : MonoBehaviour
{
    [Header("CSV Settings")]
    public string csvFileName = "data"; // data.csv

    [Header("AR")]
    public ARTrackedImageManager imageManager;

    [Header("Save Manager")]
    public CsvSaveManagerMJ saveManager;

    // CSV에서 읽은 데이터
    private List<AnchorData> anchorDataList = new List<AnchorData>();

    // 이미 생성된 오브젝트 관리 (중복 생성 방지)
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        string csvText = LoadCsvFile(csvFileName);
        if (!string.IsNullOrEmpty(csvText))
        {
            anchorDataList = ParseCsvData(csvText);
            Debug.Log($"[AnchorDataLoader] CSV 로드 완료 ({anchorDataList.Count}개)");
        }
    }

    void OnEnable()
    {
        if (imageManager != null)
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            string markerName = trackedImage.referenceImage.name;

            if (spawnedObjects.ContainsKey(markerName))
                continue;

            AnchorData data = anchorDataList.Find(d => d.anchorId == markerName);
            if (!string.IsNullOrEmpty(data.anchorId))
            {
                SpawnObjectOnMarker(trackedImage, data);
            }
        }
    }

    private void SpawnObjectOnMarker(ARTrackedImage trackedImage, AnchorData data)
    {
        GameObject prefab = Resources.Load<GameObject>(data.objectName);
        if (prefab == null)
        {
            Debug.LogWarning($"[AnchorDataLoader] 프리팹 없음: {data.objectName}");
            return;
        }

        // 마커 기준으로 생성 (마커 이동 시 같이 이동)
        GameObject instance = Instantiate(prefab, trackedImage.transform);

        instance.name = data.objectName;

        // CSV 값 적용 (로컬 기준)
        instance.transform.localPosition = data.position;
        instance.transform.localEulerAngles = data.rotation;
        instance.transform.localScale = data.scale;

        // 선택 / 조작을 위한 필수 컴포넌트
        if (instance.GetComponent<Collider>() == null)
            instance.AddComponent<BoxCollider>();

        if (instance.GetComponent<ARSelectableObject>() == null)
            instance.AddComponent<ARSelectableObject>();

        // 저장 대상 자동 등록
        if (saveManager != null)
        {
            SaveTarget st = new SaveTarget
            {
                anchorId = data.anchorId,
                objectName = data.objectName,
                target = instance.transform
            };

            saveManager.targets.Add(st);
        }

        spawnedObjects.Add(data.anchorId, instance);

        Debug.Log($"[AnchorDataLoader] 생성 완료: {data.anchorId} → {data.objectName}");
    }

    private string LoadCsvFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".csv");

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Debug.LogError($"[AnchorDataLoader] CSV 파일 없음: {path}");
            return null;
        }
    }

    private List<AnchorData> ParseCsvData(string csvText)
    {
        List<AnchorData> list = new List<AnchorData>();

        string[] lines = csvText.Split(
            new char[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        foreach (string line in lines)
        {
            if (line.StartsWith("anchor_id"))
                continue;

            string[] v = line.Split(',');

            if (v.Length < 11)
                continue;

            AnchorData data = new AnchorData
            {
                anchorId = v[0].Trim(),
                objectName = v[1].Trim(),
                position = new Vector3(
                    float.Parse(v[2]),
                    float.Parse(v[3]),
                    float.Parse(v[4])
                ),
                rotation = new Vector3(
                    float.Parse(v[5]),
                    float.Parse(v[6]),
                    float.Parse(v[7])
                ),
                scale = new Vector3(
                    float.Parse(v[8]),
                    float.Parse(v[9]),
                    float.Parse(v[10])
                )
            };

            list.Add(data);
        }

        return list;
    }
}
