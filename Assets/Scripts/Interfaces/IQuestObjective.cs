using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestObjective
{
    public void Initialize(); // 퀘스트에 필요한 정보 초기화(처치한 적 25/50)
    public void UpdateQuestProgress(object value); // 성공시 QuestClear 호출, 실패시 QuestFailed 호출
    public QuestStatus CheckQuestStatus();
    public void QuestCompleted();
    public void QuestFailed(); 
}

public enum QuestStatus
{
    InProgress,
    Completed,
    Failed
}