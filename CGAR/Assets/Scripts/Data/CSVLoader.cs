using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CSVLoader : MonoBehaviour
{
    [Tooltip("StreamingAssets 폴더 안의 CSV 파일명")]
    public string csvFileName = "data.csv";

    public IEnumerator LoadAnchorDataFromStreamingAssets(
        Action<Dictionary<string, AnchorData>> onLoaded)
    {
        Dictionary<string, AnchorData> dict = new Dictionary<string, AnchorData>();

        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

        string url = path;
        if (!url.StartsWith("http://") &&
            !url.StartsWith("https://") &&
            !url.StartsWith("file://"))
        {
            url = "file://" + url;
        }

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogError("[CSVLoader] Failed to load CSV: " + req.error);
                onLoaded?.Invoke(dict);
                yield break;
            }

            string csvText = req.downloadHandler.text;
            if (string.IsNullOrEmpty(csvText))
            {
                Debug.LogError("[CSVLoader] CSV is empty");
                onLoaded?.Invoke(dict);
                yield break;
            }

            ParseCSV(csvText, dict);
        }

        Debug.Log("[CSVLoader] Loaded rows = " + dict.Count);
        onLoaded?.Invoke(dict);
    }

    private void ParseCSV(string csvText, Dictionary<string, AnchorData> dict)
    {
        string[] lines = csvText.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        if (lines.Length <= 1)
            return;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 11)
                continue;

            string anchorId = cols[0].Trim();   // Marker 이름
            string objectName = cols[1].Trim();

            AnchorData data = new AnchorData
            {
                objectName = objectName,
                position = new Vector3(
                    ParseFloat(cols[2]),
                    ParseFloat(cols[3]),
                    ParseFloat(cols[4])
                ),
                rotation = new Vector3(
                    ParseFloat(cols[5]),
                    ParseFloat(cols[6]),
                    ParseFloat(cols[7])
                ),
                scale = new Vector3(
                    ParseFloat(cols[8]),
                    ParseFloat(cols[9]),
                    ParseFloat(cols[10])
                )
            };

            dict[anchorId] = data;
        }
    }



    private float ParseFloat(string value)
    {
        if (float.TryParse(
                value.Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out float result))
        {
            return result;
        }

        value = value.Replace(',', '.');
        if (float.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result))
        {
            return result;
        }

        return 0f;
    }
}
