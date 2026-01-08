using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using System.Globalization;


// 하나의 오브젝트와 마커 정보를 담는다
[System.Serializable]
public class SaveTarget
{
    public string anchorId;      // 마커 이름 
    public string objectName;    // 오브젝트 이름 
    public Transform target;     // 실제로 움직이는 오브젝트 트랜스폼
}

// 오브젝트 위치/회전/스케일을 csv 파일로 저장한다
public class CsvSaveManagerMJ : MonoBehaviour
{
    public List<SaveTarget> targets = new List<SaveTarget>();  // 저장할 대상 리스트

    string csvPath;   // csv 파일 경로

    //처음 시작할 때 csv 파일 경로를 설정하고 헤더가 없으면 헤더를 만든다

    
    void Awake()
    {
        csvPath = Path.Combine(Application.persistentDataPath, "data.csv");

        if (!File.Exists(csvPath))
        {
            string header =
                "anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z\n";
            File.WriteAllText(csvPath, header);
        }
    }


    //  매 프레임마다 입력을 확인하고 S 키가 눌리면 저장을 실행한다
    void Update()
    {

    }

    //  targets 리스트의 모든 오브젝트 정보를 csv 파일에 한 줄씩 추가한다
    void SaveAllTargets()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("[CsvSaveManagerMJ] 저장할 대상이 없습니다 (targets 비어 있음)");
            return;
        }

        // YG_수정파트: 기존에 덮어쓰던 방식을 이어쓰기 모드로 변경 및 모든 기능 덮어쓰기 최적화로 수정
        using (StreamWriter sw = new StreamWriter(csvPath, false)) // true: 기존 내용 뒤에 추가 ,false: 덮어쓰기
        {
            sw.WriteLine("anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z");

            foreach (var t in targets)
            {
                Vector3 pos = t.target.localPosition;
                Vector3 rot = t.target.localEulerAngles;
                Vector3 scale = t.target.localScale;

                string line = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    t.anchorId, t.objectName,
                    pos.x, pos.y, pos.z,
                    rot.x, rot.y, rot.z,
                    scale.x, scale.y, scale.z
                );


                sw.WriteLine(line);
            }
        }

        Debug.Log("[CsvSaveManagerMJ] 모든 대상 저장 완료");
    }

    public void RegisterTarget(string anchorId, string objectName, Transform target)
    {
        // 이미 등록된 항목이 있는지 확인
        SaveTarget existing = targets.Find(t =>
            t.anchorId == anchorId && t.objectName == objectName);

        if (existing != null)
        {
            // 이미 있으면 Transform만 갱신
            existing.target = target;
            return;
        }

        // 새로 등록
        targets.Add(new SaveTarget
        {
            anchorId = anchorId,
            objectName = objectName,
            target = target
        });

        Debug.Log($"[CsvSaveManagerMJ] RegisterTarget: {anchorId}, {objectName}");
    }

    public void SaveByButton()
    {
        SaveAllTargets();
    }
}
