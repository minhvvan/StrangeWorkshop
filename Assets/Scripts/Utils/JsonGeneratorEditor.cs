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

        if (GUILayout.Button("Test"))
        {
            Test();
        }
    }

    private void GenerateJson()
    {
        CompletedQuests completedQuests = new CompletedQuests();
        completedQuests.ids = new List<int>()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };
        JsonDataHandler.SaveData("completed quests", completedQuests);
        // QuestData questData = new QuestData();
        // QuestDatabase questDatabase = new QuestDatabase();
        // questDatabase.limitedTurretDatas = new List<LimitedTurretData>()
        // {
        //     new LimitedTurretData()
        // };
        // questDatabase.protectBarrierDatas = new List<ProtectBarrierData>()
        // {
        //     new ProtectBarrierData()
        // };
        // JsonDataHandler.SaveData("quests", questDatabase);
        Debug.Log("JSON 파일이 생성되었습니다");
    }

    private void Test()
    {
        // QuestDatabase a = JsonDataHandler.LoadData<QuestDatabase>("quests");
        // Debug.Log(a.quests[0].GetType());
        // ProtectBarrierQuestData b = JsonUtility.FromJson<ProtectBarrierQuestData>(JsonUtility.ToJson(a.quests[0]));
        // // JsonUtility.FromJson<QuestProtectBarrierSO>
        // // ProtectBarrierQuestData b = (ProtectBarrierQuestData)(a.quests[0]);
        // Debug.Log(b.failurePercentage);
    }
}
