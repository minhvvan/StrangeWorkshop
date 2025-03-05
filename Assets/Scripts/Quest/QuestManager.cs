using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<QuestType, IQuestObjective> _objectivesDict;
    
    // Todo: 후에 gamemanager에서 chapter 정보 받아오기
    [SerializeField] private ChapterDataSO _chapterDataSO;

    public Action OnQuestCompleted;
    public Action OnQuestFailed;
    
    void Awake()
    {
        _objectivesDict = new Dictionary<QuestType, IQuestObjective>();
        InitializeChapter();
    }
    
    public void InitializeChapter()
    {
        foreach (var quest in _chapterDataSO.quests)
        {
            _objectivesDict[quest.questType] = InitQuestObjective(quest);
        }
    }

    public void ClearEvents()
    {
        _objectivesDict.Clear();
    }

    public void Notify(QuestType questType, object param = null)
    {
        // 퀘스트와 관련있는 코드에서 QuestManager.Notify를 실행
        // 관련있는 quest가 없을 경우 실행되지 않는다
        if (_objectivesDict.TryGetValue(questType, out var objective))
        {
            objective.UpdateQuestProgress(param);
            CheckQuestStatus(questType);
        }
    }

    private IQuestObjective InitQuestObjective(QuestDataSO questData)
    {
        IQuestObjective objective = null;
        switch (questData.questType)
        {
            case QuestType.ProtectBarrier:
                objective = new ObjectiveProtectBarrier();
                break;
            case QuestType.LimitedTurret:
                objective = new ObjectiveLimitedTurret();
                break;
            default:
                break;
        }
        
        if(objective == null) Debug.LogError($"Quest {questData.questType} not found");
        
        objective.Initialize(questData);
        return objective;
    }

    private void CheckQuestStatus(QuestType questType)
    {
        if (_objectivesDict[questType].questStatus == QuestStatus.Completed)
        {
            Debug.Log("Quest completed");
            _objectivesDict.Remove(questType); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
        else if (_objectivesDict[questType].questStatus == QuestStatus.Failed)
        {
            Debug.Log("Quest Failed");
            _objectivesDict.Remove(questType); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
    }
}


public enum QuestStatus
{
    InProgress,
    Completed,
    Failed
}

public enum QuestType
{
    None,
    ProtectBarrier,
    LimitedTurret,
    Max
}
