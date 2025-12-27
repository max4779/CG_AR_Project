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

    private CsvSaveManagerMJ saveManager;
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

        saveManager = FindObjectOfType<CsvSaveManagerMJ>();
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
            //YG_수정파트: 현재 코드는 여러개의 오브젝트가 하나의 마커에 생성되는 것이 제한됨
            // 여러개 매칭되는 경우 모두 처리하도록 수정
            //AnchorData data = anchorDataList.Find(d => d.anchorId == markerName);
            List<AnchorData> matchedList = anchorDataList.FindAll(d => d.anchorId == markerName);

            if (matchedList.Count == 0)
                continue;

            // 데이터가 존재하고, 아직 오브젝트를 생성하지 않았다면 생성한다
            // YG_수정파트: 여러개의 오브젝트를 하나의 마커에 생성하도록 하기 위해 코드 수정
            // if (!string.IsNullOrEmpty(data.anchorId) && !spawnedObjects.ContainsKey(markerName))
            // {
            //     SpawnObjectOnMarker(trackedImage, data);
            // }
            foreach (var data in matchedList)
            {
                string key = markerName + "_" + data.objectName;

                // 이미 생성된 적이 있는 경우 스킵
                if (spawnedObjects.ContainsKey(key))
                    continue;

                // 생성
                GameObject instance = SpawnObjectOnMarker(trackedImage, data);

                // 관리 리스트에 저장
                spawnedObjects.Add(key, instance);

                Debug.Log($"오브젝트 생성 완료: key={key}");
            }
        } 
        
    }

    // SpawnObjectOnMarker: 마커 위에 CSV 데이터를 기반으로 오브젝트를 생성한다
    // YG_수정파트: 오브젝트 생성을 위한 아래 함수 변경
    private GameObject SpawnObjectOnMarker(ARTrackedImage trackedImage, AnchorData data)
    {
        // Resources 폴더에서 프리팹 로드 (예: Assets/Resources/toy.prefab)
        GameObject prefab = Resources.Load<GameObject>(data.objectName);

        if (prefab == null)
        {
            Debug.LogWarning($"프리팹을 찾을 수 없음: Resources/{data.objectName}");
            return null;
        }

        // 마커(trackedImage)의 자식으로 생성하여 마커가 움직이면 같이 움직이게 함
        GameObject instance = Instantiate(prefab, trackedImage.transform);
        
        // CSV 데이터 적용 (로컬 좌표 기준)
        instance.transform.localPosition = data.position;
        instance.transform.localEulerAngles = data.rotation;
        instance.transform.localScale = data.scale;
        instance.name = data.objectName;

        saveManager.RegisterTarget(data.anchorId, data.objectName, instance.transform);


        // 관리 리스트에 추가
        return instance;
    }

    // LoadCsvFile: 지정된 Assets/Data 경로에서 CSV 파일을 텍스트로 불러온다
    private string LoadCsvFile(string fileName)
    {
        // PC 에디터용 경로. (주의: 안드로이드 빌드 시에는 TextAsset 방식을 권장함)
        // YG_수정파트: 에디터용 경로 쓰는것은 좋지않음. 우리는 빌드를 목적으로 개발을 하기 때문
        //string path = Path.Combine(Application.dataPath, "Data", fileName + ".csv");
        string path = Path.Combine(Application.persistentDataPath, fileName + ".csv");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[CSV] 파일 없음: {path}");
            return null;
        }

        return File.ReadAllText(path);
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
            if (line.Contains("anchor_id")) continue;

            string[] values = line.Split(',');

            // 데이터 개수 확인 및 처리
            if (values.Length < 11) continue;

            dataList.Add(new AnchorData
            {
                anchorId = values[0].Trim(),
                objectName = values[1].Trim(),
                position = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4])),
                rotation = new Vector3(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7])),
                scale = new Vector3(float.Parse(values[8]), float.Parse(values[9]), float.Parse(values[10]))
            });
        }

        return dataList;
    }

    // CreateAnchorData: CSV 값 배열을 AnchorData 구조체로 변환한다
    // private AnchorData CreateAnchorData(string[] values)
    // {
    //     return new AnchorData
    //     {
    //         anchorId = values[0].Trim(),
    //         objectName = values[1].Trim(),
    //         position = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4])),
    //         rotation = new Vector3(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7])),
    //         scale = new Vector3(float.Parse(values[8]), float.Parse(values[9]), float.Parse(values[10]))
    //     };
    // }
}