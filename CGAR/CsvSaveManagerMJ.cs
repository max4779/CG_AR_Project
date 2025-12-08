using System.Collections.Generic;
using System.IO;
using UnityEngine;

// SaveTarget: 하나의 오브젝트와 마커 정보를 담는다
[System.Serializable]
public class SaveTarget
{
    public string anchorId;      // 마커 이름 (예: marker01)
    public string objectName;    // 오브젝트 이름 (예: Train)
    public Transform target;     // 실제로 움직이는 오브젝트 트랜스폼
}

// CsvSaveManagerMJ: 오브젝트 위치/회전/스케일을 csv 파일로 저장한다
public class CsvSaveManagerMJ : MonoBehaviour
{
    public List<SaveTarget> targets = new List<SaveTarget>();  // 저장할 대상 리스트

    string csvPath;   // csv 파일 경로

    // Start: 처음 시작할 때 csv 파일 경로를 설정하고 헤더가 없으면 헤더를 만든다
    void Start()
    {
        csvPath = Path.Combine(Application.dataPath, "Data/data.csv");

        // 파일이 없으면 헤더 한 줄만 만든다
        if (!File.Exists(csvPath))
        {
            Debug.Log($"[CsvSaveManagerMJ] csv 파일이 없어서 새로 만듭니다: {csvPath}");

            string header =
                "anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z\n";

            File.WriteAllText(csvPath, header);
        }
        else
        {
            Debug.Log($"[CsvSaveManagerMJ] 기존 csv 파일 사용: {csvPath}");
        }
    }

    // Update: 매 프레임마다 입력을 확인하고 S 키가 눌리면 저장을 실행한다
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveAllTargets();
        }
    }

    // SaveAllTargets: targets 리스트의 모든 오브젝트 정보를 csv 파일에 한 줄씩 추가한다
    void SaveAllTargets()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("[CsvSaveManagerMJ] 저장할 대상이 없습니다 (targets 비어 있음)");
            return;
        }

        using (StreamWriter sw = new StreamWriter(csvPath, true)) // true: 기존 내용 뒤에 추가
        {
            foreach (SaveTarget t in targets)
            {
                if (t == null || t.target == null)
                {
                    Debug.LogWarning("[CsvSaveManagerMJ] target 이 비어 있어서 건너뜁니다");
                    continue;
                }

                Vector3 pos = t.target.position;
                Vector3 rot = t.target.eulerAngles;
                Vector3 scale = t.target.localScale;

                string line =
                    $"{t.anchorId},{t.objectName}," +
                    $"{pos.x},{pos.y},{pos.z}," +
                    $"{rot.x},{rot.y},{rot.z}," +
                    $"{scale.x},{scale.y},{scale.z}";

                sw.WriteLine(line);

                Debug.Log($"[CsvSaveManagerMJ] 저장 완료: {line}");
            }
        }

        Debug.Log("[CsvSaveManagerMJ] 모든 대상 저장 완료");
    }
}
