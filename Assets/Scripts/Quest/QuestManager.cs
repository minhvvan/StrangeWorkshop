using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private List<QuestDataSO> availableQuests = new List<QuestDataSO>();
    
    // Todo: 후에 gamemanager에서 chapter 정보 받아오기
    [SerializeField] private ChapterDataSO _chapterDataSO;
    
    [Header("UI")]
    private InGameUIController _inGameUIController;
    public Action OnQuestCompleted;
    public Action OnQuestFailed;
    
    void Awake()
    {
        availableQuests = new List<QuestDataSO>();
        InitializeChapter();
    }
    
    async public void InitializeChapter()
    {
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
        
        foreach (var quest in _chapterDataSO.quests)
        {
            availableQuests.Add(quest);
            // InitQuestObjective(quest);
        }
        
        
    }

    public void ClearEvents()
    {
        availableQuests.Clear();
    }

    public void Notify(QuestType questType, object param = null)
    {
        // 퀘스트와 관련있는 코드에서 QuestManager.Notify를 실행
        // 관련있는 quest가 없을 경우 실행되지 않는다
        // if (_objectivesDict.TryGetValue(questType, out var objective))
        // {
        //     objective.UpdateQuestProgress(param);
        //     CheckQuestStatus(questType);
        // }
    }

    private IQuestObjective InitQuestObjective(QuestDataSO questData)
    {
        switch (questData.questType)
        {
            case QuestType.ProtectBarrier:
                questData.objective = new ObjectiveProtectBarrier();
                break;
            case QuestType.LimitedTurret:
                questData.objective = new ObjectiveLimitedTurret();
                break;
            default:
                break;
        }
        
        if(questData.objective == null) Debug.LogError($"Quest {questData.questType} not found");
        
        questData.objective.Initialize(questData);
        return questData.objective;
    }

    private void CheckQuestStatus(QuestType questType)
    {
        // if (_objectivesDict[questType].questStatus == QuestStatus.Completed)
        // {
        //     Debug.Log("Quest completed");
        //     _objectivesDict.Remove(questType); // Quest 추적을 더이상 하지 않도록 함
        //     // Todo: UI 변경을 여기 넣으면 될듯?
        // }
        // else if (_objectivesDict[questType].questStatus == QuestStatus.Failed)
        // {
        //     Debug.Log("Quest Failed");
        //     _objectivesDict.Remove(questType); // Quest 추적을 더이상 하지 않도록 함
        //     // Todo: UI 변경을 여기 넣으면 될듯?
        // }
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
