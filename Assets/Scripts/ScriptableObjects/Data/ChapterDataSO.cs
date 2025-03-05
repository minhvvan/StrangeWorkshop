using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "SO/Chapter/ChapterData")]
public class ChapterDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string chapterId;
    public string sceneName;
    public string description;
    
    [Header("Story")]
    public StoryDataSO openingStory;  // 챕터 시작 시 재생할 스토리
    public StoryDataSO endingStory;   // 챕터 클리어 시 재생할 스토리
    
    [Header("Gameplay")]
    public List<WaveDataSO> waves;    // 웨이브 정보 목록
    public float preparationTime;      // 웨이브 시작 전 준비 시간
    public Vector3 playerSpawnPoint;   // 플레이어 시작 위치
    
    [Header("Quest")]
    public List<QuestDataSO> quests;

    [Header("UI")] 
    public List<CraftRecipeSO> recommendedRecipes;
}