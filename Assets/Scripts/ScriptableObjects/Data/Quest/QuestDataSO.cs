using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestDataSO : ScriptableObject
{
    public int idx;
    public QuestType questType;
    public string description;
    [SerializeReference] public IQuestObjective objective;
}