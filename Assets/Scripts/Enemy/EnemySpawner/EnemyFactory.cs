using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


//각 SO를 읽어 데이터를 할당하는 클래스
public static class EnemyFactory
{
    //ChapterListSO를 읽어옵니다.
    public static async UniTask<ChapterDataSO> LoadChapter(ChapterListSO chapterDatas)
    {
        ChapterDataSO waveDataSo = ScriptableObject.CreateInstance<ChapterDataSO>();
        
        //현재 씬에따라 알맞는 챕터 데이터 읽어오기.
        switch (SceneManager.GetActiveScene().name)
        {
            case "Chapter1":
                waveDataSo = chapterDatas.chapters[0];
                break;
            case "Chapter2":
                waveDataSo = chapterDatas.chapters[1];
                break;
            case "IntegrationTestScene":
                waveDataSo = chapterDatas.chapters[0];
                break;
            case "HUDTest":
                waveDataSo = chapterDatas.chapters[0];
                break;
        }
        
        await UniTask.CompletedTask;
        
        return waveDataSo;
    }
    
    //WaveDataSO를 읽어옵니다.
    public static async UniTask<(int, float, List<EnemySpawnInfo>, float)> LoadWaveData(WaveDataSO waveData)
    {
        int waveNumber = 0;
        float duration = 0;
        float spawnInterval = 0;
        List<EnemySpawnInfo> enemySpawns = new List<EnemySpawnInfo>();
        
        WaveDataSO waveDataSo = ScriptableObject.CreateInstance<WaveDataSO>();
        waveDataSo = waveData;

        waveNumber = waveData.waveNumber;
        duration = waveData.duration;
        enemySpawns.AddRange(waveData.enemySpawns);
        spawnInterval = waveData.spawnInterval;

        await UniTask.CompletedTask;

        return (waveNumber, duration, enemySpawns, spawnInterval);
    }

    //EnemyDataSO를 읽어옵니다.
    public static async UniTask<(EnemyStatus, GameObject, int, int)> LoadEnemyStatus(EnemySpawnInfo enemySpawns)
    {
        EnemyStatus stats = new();
        EnemyDataSO enemyDatasSO = ScriptableObject.CreateInstance<EnemyDataSO>();
        enemyDatasSO = enemySpawns.enemyData;
        
        //읽어온 EnemyData SO 데이터를 EnemyStatus에 할당. 
        GameObject objPrefab = enemyDatasSO.enemyPrefab;
        
        stats.enemytype = enemyDatasSO.enemytype;
        stats.animFirstDelay = enemyDatasSO.animFirstDelay;
        stats.enemyName = enemyDatasSO.enemyName;
        stats.hp = enemyDatasSO.hp;
        stats.armor = enemyDatasSO.armor;
        stats.attackDamage = enemyDatasSO.attackDamage;
        stats.attackRange = enemyDatasSO.attackRange;
        stats.attackSpeed = enemyDatasSO.attackSpeed;
        stats.moveSpeed = enemyDatasSO.moveSpeed;
        
        await UniTask.CompletedTask;
        
        return (stats, objPrefab, enemySpawns.count, enemySpawns.areaID);
    }
}