using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "SO/Chapter/WaveData")]
public class WaveDataSO : ScriptableObject
{
    [Header("Wave Info")]
    public int waveNumber;
    public float duration;             // 웨이브 지속 시간
    
    [Header("Enemy Spawn")]
    public List<EnemySpawnInfo> enemySpawns;
    public float spawnInterval;        // 적 스폰 간격
}

[Serializable]
public class EnemySpawnInfo
{
    [Header("Enemy Info")]
    public EnemyDataSO enemyData;
    public int count;
}