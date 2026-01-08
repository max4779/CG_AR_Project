using System.Collections.Generic;
using UnityEngine;

public static class PrefabLoader
{
    public static Dictionary<string, GameObject> LoadAllPrefabs()
    {
        Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs");

        foreach (var prefab in prefabs)
        {
            string key = prefab.name.Trim();
            map[key] = prefab;

            Debug.Log("[PREFAB] Loaded: " + key);
        }

        Debug.Log("[PREFAB] Total = " + map.Count);
        return map;
    }
}
