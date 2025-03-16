using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Quest
{
    private QuestDataSO _questDataSO;
    private IQuestObjective _questObjective;
    
    public int id{get; private set;}
    public QuestType questType{get; private set;}
    
    public Quest(QuestDataSO questDataSO)
    {
        _questDataSO = questDataSO;
        id = _questDataSO.id;
        questType = _questDataSO.questType;
        
        InitObjective(_questDataSO.questType);
    }

    public void UpdateQuestProgress(object param = null)
    {
        _questObjective.UpdateQuestProgress(param);
    }
    
    public QuestStatus GetQuestStatus()
    {
        return _questObjective.questStatus;
    }

    public void SetQuestStatus(QuestStatus questStatus)
    {
        _questObjective.questStatus = questStatus;
    }

    public string GetQuestDescription()
    {        
        return _questObjective.description;
    }
    

    private void InitObjective(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.ProtectBarrier:
                _questObjective = new ObjectiveProtectBarrier();
                break;
            case QuestType.LimitedTurret:
                _questObjective = new ObjectiveLimitedTurret();
                break;
            default:
                Debug.LogError("No objective for {questType} quest");
                break;
        }
        _questObjective.Initialize(_questDataSO);
    }
}
