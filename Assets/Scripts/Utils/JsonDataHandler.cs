using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonDataHandler
{
    private static readonly string folderPath = Application.dataPath;
    
    public static void SaveData<T>(string fileName, T data)
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"저장 완료: {path}");
    }

    public static T LoadData<T>(string fileName)
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        if (!File.Exists(path))
        {
            Debug.Log($"파일을 찾을 수 없음: {path}");
            return default;
        }
        
        
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public static bool Exist(string fileName)
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        return File.Exists(path);
    }
}