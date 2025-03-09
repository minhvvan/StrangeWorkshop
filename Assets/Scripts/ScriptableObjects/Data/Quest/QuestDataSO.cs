using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestDataSO : ScriptableObject
{
    public int id;
    public QuestType questType;
    public string description;
    public int chapter;
    public bool cleared;
    public IQuestObjective objective;
}