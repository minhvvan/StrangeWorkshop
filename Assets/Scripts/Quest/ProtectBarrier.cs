using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectBarrier : IQuestObjective
{
    private float _percentageThreshold;
    private float _currentHealth;

    
    public void Initialize()
    {
    }

    public void UpdateQuestProgress(object value)
    {
        if (value == null) value = 0f;
        
        
    }

    public QuestStatus CheckQuestStatus()
    {
        
        return QuestStatus.InProgress;
    }

    public void QuestCompleted()
    {
        throw new NotImplementedException();
    }

    public void QuestFailed()
    {
        throw new System.NotImplementedException();
    }
}
