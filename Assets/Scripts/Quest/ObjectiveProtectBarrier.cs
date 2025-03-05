using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ObjectiveProtectBarrier : IQuestObjective
{
    private float _healthThreshold;
    private float _currentHealth;
    private BarrierStatSO _barrierStatSO;

    public QuestStatus questStatus { get; set; }

    async public void Initialize(QuestDataSO questData)
    {
        if (questData is not QuestProtectBarrierSO)
        {
            Debug.LogError("Invalid quest data");
            return;
        }
        
        _barrierStatSO = await DataManager.Instance.LoadDataAsync<BarrierStatSO>(Addresses.Data.Barrier.STAT);
    
        questStatus = QuestStatus.InProgress;
    
        _currentHealth = _barrierStatSO.totalHP;
        _healthThreshold = _currentHealth * ((QuestProtectBarrierSO)questData).failurePercentage / 100f;
    }
    

    public void UpdateQuestProgress(object value)
    {
        if (value is not float)
        {
            Debug.LogError("Invalid value type");
            return;
        }

        _currentHealth -= (float)value;
        UpdateQuestStatus();
    }

    public void UpdateQuestStatus()
    {
        // Todo: 게임 클리어시 로직 넣기
        // if(ChapterClear) _questStatus = QuestStatus.Completed;

        if (_currentHealth <= _healthThreshold) questStatus = QuestStatus.Failed;
    }
}
