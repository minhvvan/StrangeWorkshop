using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectiveLimitedTurret : IQuestObjective
{
    private int _maxTurretNum;
    private int _currentTurretNum;
    public QuestStatus questStatus { get; set; }
    public string description { get; set; }

    public void Initialize(QuestDataSO questData)
    {
        // 형변환
        if (questData is not QuestLimitedTurretSO)
        {
            Debug.LogError($"{questData} is not a QuestLimitedTurretSO");
            return;
        }
        _maxTurretNum = ((QuestLimitedTurretSO)questData).maxTurretNum;
        
        questStatus = QuestStatus.InProgress;
        description = questData.description;
    }

    public void UpdateQuestProgress(object value)
    {
        if (value is not int)
        {
            Debug.LogError("Invalid value type");
            return;
        }
        _currentTurretNum += (int)value;
        UpdateQuestStatus();
        UpdateDescription();
    }

    public void UpdateQuestStatus()
    {
        // 게임 끝날 때 로직 넣기
        // if(gameClear) questStatus = QuestStatus.Completed;
        
        if (_currentTurretNum > _maxTurretNum) questStatus = QuestStatus.Failed;
    }

    public void UpdateDescription()
    {
        description = $"터렛을 {_maxTurretNum}개 이하로 유지하세요\n{_currentTurretNum}/{_maxTurretNum}";
    }
}
