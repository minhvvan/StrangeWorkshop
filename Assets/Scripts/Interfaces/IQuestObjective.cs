using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestObjective
{
    public QuestStatus questStatus { get; set; } // 챕터 클리어와 관계없이 퀘스트 상태를 따라감
    public string description { get; set; } // questDataSO의 description과 달리 퀘스트 진행에 따라 달라짐
    public void Initialize(QuestDataSO questData); // 퀘스트에 필요한 정보 초기화
    public void UpdateQuestProgress(object value); // value를 받아 quest 진행도 업데이트
    public void UpdateQuestStatus(); // 퀘스트 성공, 실패 로직
    public void UpdateDescription();
}