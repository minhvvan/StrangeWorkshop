using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "SO/Quest/QuestData")]
public class QuestDataSO : ScriptableObject
{
    public int questID;
    public QuestType questType;
    public string questDescription;
    public IQuestObjective questObjective;
}

public enum QuestType
{
    None,
    ProtectBarrier,
    
    Max
}
