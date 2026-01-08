// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;

// public class temp : MonoBehaviour
// {
//     public ARTrackedImageManager arTrackedImageManager;

//     private GameObject[] prefabs = new GameObject[7];

//     private GameObject markerObj1, markerObj2, markerObj3, markerObj4, markerObj5, markerObj6, markerObj7;

//     void Awake()
//     {
//         // ✅ persistentDataPath에 CSV 보장
//         GameDataInitializer.EnsureCSVExists("data.csv");

//         // ✅ persistentDataPath 기준으로 CSV 로딩
//         Dictionary<string, string> prefabMap =
//             CSVLoader.LoadAnchorPrefabNames("data.csv");

//         Debug.Log("[AR] prefabMap count = " + prefabMap.Count);

//         prefabs[0] = LoadPrefab(prefabMap, "Marker1");
//         prefabs[1] = LoadPrefab(prefabMap, "Marker2");
//         prefabs[2] = LoadPrefab(prefabMap, "Marker3");
//         prefabs[3] = LoadPrefab(prefabMap, "Marker4");
//         prefabs[4] = LoadPrefab(prefabMap, "Marker5");
//         prefabs[5] = LoadPrefab(prefabMap, "Marker6");
//         prefabs[6] = LoadPrefab(prefabMap, "Marker7");
//     }

//     GameObject LoadPrefab(Dictionary<string, string> map, string marker)
//     {
//         if (!map.ContainsKey(marker))
//         {
//             Debug.LogError("[AR] CSV missing marker: " + marker);
//             return null;
//         }

//         string prefabName = map[marker].Trim();

//         Debug.Log("[AR] Try load prefab: " + prefabName);

//         GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
//         if (prefab == null)
//         {
//             Debug.LogError("[AR] Prefab not found: " + prefabName);
//         }

//         return prefab;
//     }

//     void OnEnable()
//     {
//         arTrackedImageManager.trackablesChanged.AddListener(OnChangeTrackingState);
//     }

//     void OnDisable()
//     {
//         arTrackedImageManager.trackablesChanged.RemoveListener(OnChangeTrackingState);
//     }

//     private void OnChangeTrackingState(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
//     {
//         foreach (ARTrackedImage trackedImage in eventArgs.added)
//         {
//             string name = trackedImage.referenceImage.name;

//             if (name == "Marker1" && markerObj1 == null) markerObj1 = Create(prefabs[0]);
//             if (name == "Marker2" && markerObj2 == null) markerObj2 = Create(prefabs[1]);
//             if (name == "Marker3" && markerObj3 == null) markerObj3 = Create(prefabs[2]);
//             if (name == "Marker4" && markerObj4 == null) markerObj4 = Create(prefabs[3]);
//             if (name == "Marker5" && markerObj5 == null) markerObj5 = Create(prefabs[4]);
//             if (name == "Marker6" && markerObj6 == null) markerObj6 = Create(prefabs[5]);
//             if (name == "Marker7" && markerObj7 == null) markerObj7 = Create(prefabs[6]);
//         }

//         foreach (ARTrackedImage trackedImage in eventArgs.updated)
//         {
//             string name = trackedImage.referenceImage.name;

//             UpdateObj(markerObj1, name == "Marker1", trackedImage);
//             UpdateObj(markerObj2, name == "Marker2", trackedImage);
//             UpdateObj(markerObj3, name == "Marker3", trackedImage);
//             UpdateObj(markerObj4, name == "Marker4", trackedImage);
//             UpdateObj(markerObj5, name == "Marker5", trackedImage);
//             UpdateObj(markerObj6, name == "Marker6", trackedImage);
//             UpdateObj(markerObj7, name == "Marker7", trackedImage);
//         }
//     }

//     GameObject Create(GameObject prefab)
//     {
//         if (prefab == null) return null;
//         GameObject obj = Instantiate(prefab);
//         obj.SetActive(false);
//         return obj;
//     }

//     void UpdateObj(GameObject obj, bool match, ARTrackedImage trackedImage)
//     {
//         if (!match || obj == null) return;

//         obj.transform.SetPositionAndRotation(
//             trackedImage.transform.position,
//             trackedImage.transform.rotation
//         );
//         obj.SetActive(true);
//     }
// }
