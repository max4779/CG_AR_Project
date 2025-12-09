using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.IO;

public class AnchorDataLoader : MonoBehaviour
{
    [Header("Settings")]
    public string csvFileName = "data";
    public ARTrackedImageManager imageManager; // Inspector에서 할당

    // 파싱된 데이터를 저장할 리스트
    private List<AnchorData> anchorDataList = new List<AnchorData>();
    
    // 생성된 오브젝트를 관리하기 위한 딕셔너리 (마커 이름 -> 생성된 오브젝트)
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    // Awake: 초기화 시 CSV 파일을 읽고 데이터를 파싱한다
    void Awake()
    {
        // 1. CSV 파일 로드 및 파싱 (앱 시작 시 한 번만 수행)
        string csvText = LoadCsvFile(csvFileName);
        if (!string.IsNullOrEmpty(csvText))
        {
            anchorDataList = ParseCsvData(csvText);
            Debug.Log($"CSV 데이터 로드 완료: {anchorDataList.Count}개의 데이터");
        }
    }

    // OnEnable: AR 이미지 추적 이벤트를 구독한다
    void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    // OnDisable: AR 이미지 추적 이벤트를 구독 해지한다
    void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    // OnTrackedImagesChanged: 마커가 인식되거나 상태가 변할 때 호출된다
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 새로 추가된 마커(이미지)에 대해 처리
        foreach (var trackedImage in eventArgs.added)
        {
            string markerName = trackedImage.referenceImage.name;
            
            // CSV 데이터 리스트에서 이 마커 이름과 일치하는 데이터를 찾는다
            AnchorData data = anchorDataList.Find(d => d.anchorId == markerName);

            // 데이터가 존재하고, 아직 오브젝트를 생성하지 않았다면 생성한다
            if (!string.IsNullOrEmpty(data.anchorId) && !spawnedObjects.ContainsKey(markerName))
            {
                SpawnObjectOnMarker(trackedImage, data);
            }
        } 
        
    }

    // SpawnObjectOnMarker: 마커 위에 CSV 데이터를 기반으로 오브젝트를 생성한다
    private void SpawnObjectOnMarker(ARTrackedImage trackedImage, AnchorData data)
    {
        // Resources 폴더에서 프리팹 로드 (예: Assets/Resources/toy.prefab)
        GameObject prefab = Resources.Load<GameObject>(data.objectName);

        if (prefab == null)
        {
            Debug.LogWarning($"프리팹을 찾을 수 없음: Resources/{data.objectName}");
            return;
        }

        // 마커(trackedImage)의 자식으로 생성하여 마커가 움직이면 같이 움직이게 함
        GameObject instance = Instantiate(prefab, trackedImage.transform);

        // CSV 데이터 적용 (로컬 좌표 기준)
        instance.transform.localPosition = data.position;
        instance.transform.localEulerAngles = data.rotation;
        instance.transform.localScale = data.scale;
        instance.name = data.objectName;

        // 관리 리스트에 추가
        spawnedObjects.Add(data.anchorId, instance);

        Debug.Log($"오브젝트 생성 완료: {data.anchorId} -> {data.objectName}");
    }

    // LoadCsvFile: 지정된 Assets/Data 경로에서 CSV 파일을 텍스트로 불러온다
    private string LoadCsvFile(string fileName)
    {
        // PC 에디터용 경로. (주의: 안드로이드 빌드 시에는 TextAsset 방식을 권장함)
        string path = Path.Combine(Application.dataPath, "Data", fileName + ".csv");
        
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Debug.LogError($"파일 없음: {path}");
            return null;
        }
    }

    private List<AnchorData> ParseCsvData(string csvText)
    {
        List<AnchorData> dataList = new List<AnchorData>();

        // StringSplitOptions.RemoveEmptyEntries: 빈 줄은 알아서 삭제한다
        string[] lines = csvText.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        Debug.Log($"[전체 줄 수 확인]: 총 {lines.Length}줄이 감지되었습니다.");

        // 2. 한 줄씩 검사
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            // "anchor_id"로 시작하면 헤더로 간주하고 패스
            if (line.Contains("anchor_id") || string.IsNullOrWhiteSpace(line)) 
            {
                Debug.Log($"[헤더 또는 빈 줄 스킵]: {line}");
                continue;
            }

            string[] values = line.Split(',');

            // 데이터 개수 확인 및 처리
            if (values.Length >= 11)
            {
                dataList.Add(CreateAnchorData(values));
                Debug.Log($"[데이터 로드 성공]: {values[1]} (줄 {i+1})");
            }
            else
            {
                Debug.LogWarning($"[데이터 개수 부족]: {line} (현재 {values.Length}개 / 11개 필요)");
            }
        }

        return dataList;
    }

    // CreateAnchorData: CSV 값 배열을 AnchorData 구조체로 변환한다
    private AnchorData CreateAnchorData(string[] values)
    {
        return new AnchorData
        {
            anchorId = values[0].Trim(),
            objectName = values[1].Trim(),
            position = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4])),
            rotation = new Vector3(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7])),
            scale = new Vector3(float.Parse(values[8]), float.Parse(values[9]), float.Parse(values[10]))
        };
    }
}