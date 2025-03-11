using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class JsonDataHandler
{
    private static string folderPath
    {
        get
        {
            if (Application.isEditor) return Path.Combine(Application.dataPath, "Data", "Json");
            else return Application.persistentDataPath;
        }
    }
    
    public static async UniTask SaveData<T>(string fileName, T data)
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        string json = JsonUtility.ToJson(data, true);
        await File.WriteAllTextAsync(path, json);
        Debug.Log($"저장 완료: {path}");
        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    public static async UniTask<T> LoadData<T>(string fileName) where T : new()
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        if (!File.Exists(path)) // 파일이 없으면 기본 데이터로 생성
        {
            Debug.Log($"파일이 없습니다. 기본 데이터로 생성합니다: {path}");
            T data = new T();
            await SaveData<T>(fileName, data);
            return data;
        }
        string json = await File.ReadAllTextAsync(path);
        return JsonUtility.FromJson<T>(json);
    }

    public static bool Exist(string fileName)
    {
        string path = Path.Combine(folderPath, fileName + ".json");
        return File.Exists(path);
    }
}