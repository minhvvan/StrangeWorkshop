using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveLimitedTurret : IQuestObjective
{
    private int _maxTurretNum;
    private int _currentTurretNum;
    public QuestStatus questStatus { get; set; }
    
    public void Initialize(QuestDataSO questData)
    {
        // 형변환
        if (questData is not QuestLimitedTurretSO)
        {
            Debug.LogError("Invalid quest data");
            return;
        }
        _maxTurretNum = ((QuestLimitedTurretSO)questData).maxTurretNum;

        questStatus = QuestStatus.InProgress;
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
    }

    public void UpdateQuestStatus()
    {
        // 게임 끝날 때 로직 넣기
        // if(gameClear) questStatus = QuestStatus.Completed;
        
        if (_currentTurretNum > _maxTurretNum) questStatus = QuestStatus.Failed;
        
        
    }
}
