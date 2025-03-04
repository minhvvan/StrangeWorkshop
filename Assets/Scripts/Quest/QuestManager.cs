using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<QuestType, Action<object>> eventsDict;
    
    [SerializeField] private ChapterDataSO _chapterDataSO;
    
    void Awake()
    {
        eventsDict = new Dictionary<QuestType, Action<object>>();
    }
    
    public void InitializeChapter(int chapterIdx)
    {
        foreach (var quest in _chapterDataSO.quests)
        {
            eventsDict[quest.questType] = quest.questObjective.UpdateQuestProgress;
        }
    }

    public void ClearEvents()
    {
        eventsDict.Clear();
    }

    public void Notify(QuestType questType, object param = null)
    {
        // 관련있는 quest가 없을 경우 실행되지 않는다
        if (eventsDict.TryGetValue(questType, out var action))
        {
            action?.Invoke(param);
        }
    }
}
