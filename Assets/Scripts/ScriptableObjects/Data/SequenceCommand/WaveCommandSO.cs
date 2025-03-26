using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "SO/Chapter/WaveData")]
public class WaveCommandSO : SequenceCommandSO
{
    [Header("Wave Info")]
    public int waveNumber;
    public float duration;             // 웨이브 지속 시간

    public enum WaveType
    {
        Common,
        Boss
    }
    
    public WaveType waveType;
    
    [Header("Enemy Spawn")]
    public List<EnemySpawnInfo> enemySpawns;
    public float spawnInterval;        // 적 스폰 간격
    private bool IsCompleted { get; set; }

    private SpawnEnemyEventSO _spawnEventSO;
    private BossEndEventSO _bossEndEventSO;
    private WaveClearEventSO _waveClearEventSO;
    private WaveStartEventSO _waveStartEventSO;

    private CommandCompleteEventSO so;
    
    public override async void Initialize()
    {
        IsInitialized = false;
        IsCompleted = false;
        
        _spawnEventSO = await DataManager.Instance.LoadDataAsync<SpawnEnemyEventSO>(Addresses.Events.Game.ENEMY_SPAWN);
        _waveStartEventSO = await DataManager.Instance.LoadDataAsync<WaveStartEventSO>(Addresses.Events.Game.WAVE_START);
        _waveClearEventSO = await DataManager.Instance.LoadDataAsync<WaveClearEventSO>(Addresses.Events.Game.WAVE_CLEAR);
        _bossEndEventSO = await DataManager.Instance.LoadDataAsync<BossEndEventSO>(Addresses.Events.Game.BOSS_END);
        
        IsInitialized = true;
    }

    public override async void Execute(CommandCompleteEventSO completeEventSo)
    {
        await UniTask.WaitUntil(() => IsInitialized);
        
        //준비 시간
        _waveStartEventSO.Raise(startDelay);
        await UniTask.Delay(TimeSpan.FromSeconds(startDelay));
        
        //Spawner에서 적을 생성하도록 호출
        await StartWave();

        if (waveType == WaveType.Boss)
        {
            _bossEndEventSO.AddListener(BossEnd);
            await UniTask.WaitUntil(()=>IsCompleted);
        }
        
        completeEventSo.Raise();
        _waveClearEventSO.Raise();
    }

    private async UniTask StartWave()
    {
        float currentTime = 0f;
        float spawnTime = 0;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                _spawnEventSO.Raise(enemySpawns);
                spawnTime = spawnInterval;
            }
            
            await UniTask.Yield();
        }
    }

    private void BossEnd()
    {
        IsCompleted = true;
        _bossEndEventSO.RemoveListener(BossEnd);
    }
}

[Serializable]
public class EnemySpawnInfo
{
    [Header("Enemy Info")]
    public EnemyDataSO enemyData;
    public int count;
    public int areaID;
}