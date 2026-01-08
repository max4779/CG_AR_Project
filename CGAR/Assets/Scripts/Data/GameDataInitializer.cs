using System.IO;
using UnityEngine;

public static class GameDataInitializer
{
    public static void EnsureCSVExists(string fileName)
    {
        string targetPath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(targetPath))
            return;

        TextAsset csv = Resources.Load<TextAsset>("Data/" + Path.GetFileNameWithoutExtension(fileName));
        if (csv == null)
        {
            Debug.LogError("Initial CSV not found in Resources/Data");
            return;
        }

        File.WriteAllText(targetPath, csv.text);
        Debug.Log("CSV copied to persistentDataPath: " + targetPath);
    }
}
