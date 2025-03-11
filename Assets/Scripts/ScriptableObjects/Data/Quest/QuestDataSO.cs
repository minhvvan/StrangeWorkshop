using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestDataSO : ScriptableObject
{
    public int id;
    public QuestType questType;
    [Multiline]
    public string description;
    public int chapter;
}