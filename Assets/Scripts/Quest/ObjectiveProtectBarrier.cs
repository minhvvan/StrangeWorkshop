using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

[Serializable]
public class ObjectiveProtectBarrier : IQuestObjective
{
    [SerializeField]private float _healthThreshold;
    private float _currentHealth;
    private BarrierStatSO _barrierStatSO;
    private float _failurePercentage;
    
    public QuestStatus questStatus { get; set; }
    public string description { get; set; } // UI용

    async public void Initialize(QuestDataSO questData)
    {
        if (questData is not QuestProtectBarrierSO)
        {
            Debug.LogError($"{questData} is not a QuestProtectBarrierSO");
            return;
        }
        
        _barrierStatSO = await DataManager.Instance.LoadDataAsync<BarrierStatSO>(Addresses.Data.Barrier.STAT);
        
        questStatus = QuestStatus.InProgress;
        description = questData.description;
    
        _currentHealth = _barrierStatSO.totalHP;
        _failurePercentage = ((QuestProtectBarrierSO)questData).failurePercentage;
        _healthThreshold = _currentHealth * _failurePercentage / 100f;
    }
    

    public void UpdateQuestProgress(object value)
    {
        // value: 체력 감소량
        if (value is not float)
        {
            Debug.LogError("Invalid value type");
            return;
        }

        _currentHealth -= (float)value;
        UpdateQuestStatus();
        UpdateDescription();
    }

    public void UpdateQuestStatus()
    {
        // Todo: 게임 클리어시 로직 넣기
        // if(ChapterClear) _questStatus = QuestStatus.Completed;

        if (_currentHealth <= _healthThreshold) questStatus = QuestStatus.Failed;
    }

    public void UpdateDescription()
    {
        description = $"방벽의 체력을 {_failurePercentage}% 이상으로\n유지하세요\n{_currentHealth}/{_barrierStatSO.totalHP}";
    }
}
