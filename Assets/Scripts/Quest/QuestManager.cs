using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Serialization;

// todo: save, turretmaxobjective complete 로직 수정

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<int, List<QuestDataSO>> _questDataSOsByChapter;
    
    // 퀘스트는 퀘스트 달성 + 챕터 클리어가 동반되어야 클리어로 인정된다
    // e.g. 챕터 중간에 퀘스트를 달성해도 해당 챕터를 클리어하지 못했다면 다시 깨야함
    // completedQuestIDset은 챕터 클리어 이후 업데이트
    private SortedSet<int> _completedQuestIdsSet; 
    
    public List<Quest> availableQuests { get; private set; }
    
    public bool IsInitialized { get; private set; }
    
    [Header("UI")]
    private InGameUIController _inGameUIController;

    public Action<Quest> OnQuestProgressUpdated;
    

    async void Start()
    {
        availableQuests = new List<Quest>();

        Initialize();
    }

    public async UniTask Initialize()
    {
        // await UniTask.WaitUntil(() => DataManager.Instance.IsIn
        QuestListSO questListSO = await DataManager.Instance.LoadDataAsync<QuestListSO>(Addresses.Data.Quest.QUESTLIST);
        await LoadCompletedQuests(questListSO);
        GroupingQuests(questListSO);
        
        await InitializeChapter(1);
        IsInitialized = true;
    }
    
    public async UniTask InitializeChapter(int chapterIdx)
    {
        // Chapter 시작때 호출
        // availableQuests 초기화
        foreach (var questDataSO in _questDataSOsByChapter[chapterIdx])
        {
            Quest quest = new Quest(questDataSO);
            if( _completedQuestIdsSet.Contains(quest.id))
            {
                quest.SetQuestStatus(QuestStatus.Completed);
            }
            availableQuests.Add(quest);
        }
        
        // 챕터가 시작될 때마다 호출된다.
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
    }
    
    public void Notify(QuestType questType, object param = null)
    {
        for (int i = availableQuests.Count - 1; i>=0; i --)
        {
            Quest quest = availableQuests[i];
            // 퀘스트의 클리어여부와 관련있는 코드에서 QuestManager.Notify를 실행
            // 관련있는 quest가 해당 챕터에 없을 경우 실행되지 않는다
            if (quest.questType == questType && quest.GetQuestStatus() != QuestStatus.Completed)
            {
                quest.UpdateQuestProgress(param);
                CheckQuestStatus(quest);
                OnQuestProgressUpdated?.Invoke(quest);
            }
        }
    }

    async public void EndChapter()
    {
        foreach (var quest in availableQuests)
        {
            if(quest.GetQuestStatus() == QuestStatus.Completed)
                _completedQuestIdsSet.Add(quest.id);
        }
        
        SaveCompletedQuests();
        
        availableQuests.Clear();
    }

    async private void SaveCompletedQuests()
    {
        CompletedQuests completedQuests = new CompletedQuests(_completedQuestIdsSet.ToList());
        await JsonDataHandler.SaveData("completed quests", completedQuests);
    }

    private void CheckQuestStatus(Quest quest)
    {
        if (quest.GetQuestStatus() == QuestStatus.Completed)
        {
            availableQuests.Remove(quest); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
        else if (quest.GetQuestStatus() == QuestStatus.Failed)
        {
            availableQuests.Remove(quest); // Quest 추적을 더이상 하지 않도록 함
            // Todo: UI 변경을 여기 넣으면 될듯?
        }
    }

    private void GroupingQuests(QuestListSO questList)
    {
        // 챕터별 quest로 dictionary화
        _questDataSOsByChapter = questList.questDataSOs
            .Select(q=>q)
            .GroupBy(q => q.chapter) // 챕터별로 그룹화
            .ToDictionary(g => g.Key, g => g.ToList()); // Dictionary<int, List<QuestData>> 형태로 변환
    }
    
    private async UniTask LoadCompletedQuests(QuestListSO questList)
    {
        // 클리어한 챕터 id 불러오기
        CompletedQuests q = await JsonDataHandler.LoadData<CompletedQuests>("completed quests");
        _completedQuestIdsSet = new SortedSet<int>(q.ids);
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