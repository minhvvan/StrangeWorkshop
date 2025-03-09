using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<int, List<QuestDataSO>> _questDataSOsByChapter;
    
    public List<int> completedQuests;

    private List<QuestDataSO> _availableQuests;
    
    // Todo: 후에 gamemanager에서 chapter 정보 받아오기
    [SerializeField] private ChapterDataSO _chapterDataSO;
    
    [Header("UI")]
    private InGameUIController _inGameUIController;
    public Action OnQuestCompleted;
    public Action OnQuestFailed;
    

    async void Start()
    {
        _availableQuests = new List<QuestDataSO>();
        
        await Initialize();
    }

    public async UniTask Initialize()
    {
        // await UniTask.WaitUntil(() => DataManager.Instance.IsIn
        QuestListSO questListSO = await DataManager.Instance.LoadDataAsync<QuestListSO>(Addresses.Data.Quest.QUESTLIST);
        await LoadCompletedQuests();
        GroupingQuests(questListSO);
    }
    
    async public void InitializeChapter(int chapterIdx)
    {
        // 챕터가 시작될 때마다 호출된다.
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
        
        foreach (var questDataSO in _questDataSOsByChapter[chapterIdx])
        {
            if (!questDataSO.cleared)
            {
                _availableQuests.Add(questDataSO);
                InitQuestObjective(questDataSO);
            }
        }
    }

    public void ClearEvents()
    {
        _availableQuests.Clear();
    }

    
    public void Notify(QuestType questType, object param = null)
    {
        for (int i = _availableQuests.Count - 1; i>=0; i --)
        {
            QuestDataSO questData = _availableQuests[i];
            // 퀘스트의 클리어여부와 관련있는 코드에서 QuestManager.Notify를 실행
            // 관련있는 quest가 해당 챕터에 없을 경우 실행되지 않는다
            if (questData.questType == questType)
            {
                questData.objective.UpdateQuestProgress(param);
                CheckQuestStatus(questData);
            }
        }
    }

    private void InitQuestObjective(QuestDataSO questData)
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
    }

    private void CheckQuestStatus(QuestDataSO questData)
    {
        if (questData.objective.questStatus == QuestStatus.Completed)
        {
            Debug.Log("Quest completed");
            _availableQuests.Remove(questData); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
        else if (questData.objective.questStatus == QuestStatus.Failed)
        {
            Debug.Log("Quest Failed");
            _availableQuests.Remove(questData); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
    }

    private void GroupingQuests(QuestListSO questList)
    {
        _questDataSOsByChapter = questList.questDataSOs
            .Select(q=>q)
            .GroupBy(q => q.chapter) // 챕터별로 그룹화
            .ToDictionary(g => g.Key, g => g.ToList()); // Dictionary<int, List<QuestData>> 형태로 변환
    }
    
    private async UniTask LoadCompletedQuests()
    {
        // completedQuests = 
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