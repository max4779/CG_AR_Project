using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OneShotAnchorCsvExporter : MonoBehaviour
{
    [Header("위에서부터 Marker1, Marker2, Marker3 ... 순서")]
    public List<GameObject> prefabsInOrder;

    [Header("CSV 파일 이름 (확장자 제외)")]
    public string csvFileName = "data";

    void Start()
    {
        ExportCsv();
        Debug.Log("CSV 저장 완료 - 이 스크립트는 이제 삭제해도 됩니다.");
    }

    void ExportCsv()
    {
#if UNITY_EDITOR
        // Assets/Resources/Data 경로
        string folderPath = Path.Combine(
            Application.dataPath,
            "Resources",
            "Data"
        );

        // 폴더 없으면 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string path = Path.Combine(folderPath, csvFileName + ".csv");

        // 기존 파일 있으면 삭제 (완전 덮어쓰기)
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            // CSV Header
            writer.WriteLine(
                "anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z"
            );

            for (int i = 0; i < prefabsInOrder.Count; i++)
            {
                GameObject obj = prefabsInOrder[i];
                if (obj == null)
                    continue;

                string markerId = $"Marker{i + 1}";
                string objectName = obj.name;

                Vector3 pos = obj.transform.localPosition;
                Vector3 rot = obj.transform.localEulerAngles;
                Vector3 scale = obj.transform.localScale;

                string line =
                    $"{markerId},{objectName}," +
                    $"{pos.x},{pos.y},{pos.z}," +
                    $"{rot.x},{rot.y},{rot.z}," +
                    $"{scale.x},{scale.y},{scale.z}";

                writer.WriteLine(line);
            }
        }

        // Project 창에 즉시 반영
        AssetDatabase.Refresh();

        Debug.Log($"CSV 생성 위치: {path}");
#else
        Debug.LogWarning("OneShotAnchorCsvExporter는 Unity Editor 전용입니다.");
#endif
    }
}
