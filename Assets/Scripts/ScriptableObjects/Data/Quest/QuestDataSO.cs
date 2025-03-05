using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestDataSO : ScriptableObject
{
    public int questID;
    public QuestType questType;
    public string questDescription;
}