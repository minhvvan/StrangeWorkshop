using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestObjective
{
    public QuestType questType { get; set; }
    public QuestStatus questStatus { get; set; }
    public void Initialize(QuestDataSO questData); // 퀘스트에 필요한 정보 초기화
    public void UpdateQuestProgress(object value); // value를 받아 quest 진행도 업데이트
    public void UpdateQuestStatus(); // 퀘스트 성공, 실패 로직
}