using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class CSVLoader : MonoBehaviour
{
    public IEnumerator LoadAnchorDataFromStreamingAssets(
        System.Action<Dictionary<string, AnchorData>> onLoaded)
    {
        Dictionary<string, AnchorData> dict = new Dictionary<string, AnchorData>();

        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            "data.csv"
        );

        Debug.Log("[CSV] try load: " + path);

        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[CSV] Load failed: " + req.error);
            onLoaded?.Invoke(dict);
            yield break;
        }

        string text = req.downloadHandler.text;
        string[] lines = text.Split('\n');

        Debug.Log("[CSV] line count = " + lines.Length);

        if (lines.Length < 2)
        {
            onLoaded?.Invoke(dict);
            yield break;
        }

        // 탭 / 쉼표 자동 판별
        char delimiter = lines[1].Contains("\t") ? '\t' : ',';

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] t = lines[i].Trim().Split(delimiter);
            if (t.Length < 11)
            {
                Debug.LogError("[CSV] Invalid line: " + lines[i]);
                continue;
            }

            AnchorData data = new AnchorData
            {
                anchorId = t[0].Trim(),
                objectName = t[1].Trim(),
                position = new Vector3(
                    float.Parse(t[2], CultureInfo.InvariantCulture),
                    float.Parse(t[3], CultureInfo.InvariantCulture),
                    float.Parse(t[4], CultureInfo.InvariantCulture)
                ),
                rotation = new Vector3(
                    float.Parse(t[5], CultureInfo.InvariantCulture),
                    float.Parse(t[6], CultureInfo.InvariantCulture),
                    float.Parse(t[7], CultureInfo.InvariantCulture)
                ),
                scale = new Vector3(
                    float.Parse(t[8], CultureInfo.InvariantCulture),
                    float.Parse(t[9], CultureInfo.InvariantCulture),
                    float.Parse(t[10], CultureInfo.InvariantCulture)
                )
            };

            dict[data.anchorId] = data;
        }

        Debug.Log("[CSV] Loaded anchor count = " + dict.Count);
        onLoaded?.Invoke(dict);
    }
}
