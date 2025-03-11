using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class JsonGeneratorEditor : EditorWindow
{
    [MenuItem("Tools/Generate JSON")]
    public static void ShowWindow()
    {
        GetWindow<JsonGeneratorEditor>("Json Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate JSON");
        GUILayout.Space(10);
        GUILayout.Label("Test");

        if (GUILayout.Button("Generate JSON"))
        {
            GenerateJson();
        }
    }

    private void GenerateJson()
    {
        // json sample 생성
        //JsonDataHandler.SaveData("[filename]", [data]);
        Debug.Log("JSON 파일이 생성되었습니다");
    }
}
