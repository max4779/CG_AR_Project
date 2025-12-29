using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class VRSaveTarget
{
    public string anchorId;      
    public string objectName;    
    public Transform target;     
}

public class VRPlacementSaver : MonoBehaviour
{
    public List<VRSaveTarget> targets = new List<VRSaveTarget>();

    private string csvPath;
    private InputDevice rightController;
    private InputDevice leftController;

    void Start()
    {
        // Quest3에서 쓰기 가능한 경로
        csvPath = Path.Combine(Application.persistentDataPath, "data.csv");
        Debug.Log("[VRPlacementSaver] CSV 저장 경로: " + csvPath);

        // 컨트롤러 가져오기
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        leftController  = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // CSV 없으면 헤더 생성
        if (!File.Exists(csvPath))
        {
            string header =
                "anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z\n";

            File.WriteAllText(csvPath, header);
            Debug.Log("[VRPlacementSaver] CSV 새로 생성됨");
        }
    }

    void Update()
    {
        // 오른손 A 버튼
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            SaveToCsv();
        }

        // 왼손 X 버튼
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed) && xPressed)
        {
            SaveToCsv();
        }
    }

    private void SaveToCsv()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("[VRPlacementSaver] 저장할 대상이 없습니다.");
            return;
        }

        using (StreamWriter sw = new StreamWriter(csvPath, false)) // CSV 덮어쓰기
        {
            sw.WriteLine("anchor_id,object_name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,scale_x,scale_y,scale_z");

            foreach (var t in targets)
            {
                Vector3 pos = t.target.localPosition;
                Vector3 rot = t.target.localEulerAngles;
                Vector3 scale = t.target.localScale;

                string line =
                    $"{t.anchorId},{t.objectName}," +
                    $"{pos.x},{pos.y},{pos.z}," +
                    $"{rot.x},{rot.y},{rot.z}," +
                    $"{scale.x},{scale.y},{scale.z}";

                sw.WriteLine(line);
            }
        }

        Debug.Log("[VRPlacementSaver] CSV 저장 완료!");
    }


    // YG: VR사용을 위한 타겟 등록 함수
    public void RegisterTarget(string anchorId, string objectName, Transform target)
    {
        foreach (var t in targets)
        {
            if (t.anchorId == anchorId && t.objectName == objectName)
                return;
        }

        targets.Add(new VRSaveTarget
        {
            anchorId = anchorId,
            objectName = objectName,
            target = target
        });

        Debug.Log("[VRPlacementSaver] 등록됨 → " + anchorId + " / " + objectName);
    }

}
