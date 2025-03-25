using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class InGameDataController : MonoBehaviour
{
    private bool _playing = false;
    
    [Header("Turret")] 
    private Dictionary<TurretType, int> _turretCounts;

    [Header("Enemy")] 
    private Dictionary<EnemyType, int> _killedEnemyCounts;

    [Header("Barrier")] 
    private float _remainingBarrierCount;
    private float _remainingTotalHP;
    
    [Header("Gold")]
    private int _gold;
    private float _interval = 1f;
    private int _goldPerInterval = 1;

    [Header("Time")] 
    private float _chapterPlayTime;
    
    [Header("Crafting")]
    private Dictionary<PartMaterialType, int> _partMaterialCounts;

    [Header("Wave")] 
    private float _currentWave;
    
    public void Initialize()
    {
    }

    public void StartGame()
    {
        _playing = true;
        // todo: 처음 설치된 turret 개수 불러오기
        // _turretCounts = new Dictionary<TurretType, int>();
        
        _killedEnemyCounts = new Dictionary<EnemyType, int>();
        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            _killedEnemyCounts[enemyType] = 0;
        }

        CheckPlayTime();
        EarnGoldContinuously();
    }

    public void StopGame()
    {
        _playing = false;
    }

    public void ModifyInGameState<T>(InGameDataType dataType, object value, [CanBeNull] object subType = null)
    {
        switch (dataType)
        {
            case InGameDataType.TurretCount:
                if (subType is TurretType)
                    _turretCounts[(TurretType)subType] += (int)value;
                break;
            case InGameDataType.KilledEnemy:
                if(subType is EnemyType)
                    _killedEnemyCounts[(EnemyType)subType] += (int)value;
                break;
            case InGameDataType.BarrierCount:
                _remainingBarrierCount += (int)value;
                break;
            case InGameDataType.TotalHP:
                _remainingTotalHP += (int)value;
                break;
            case InGameDataType.PartMaterialCount:
                if(subType is PartMaterialType)
                    _partMaterialCounts[(PartMaterialType)subType] += (int)value;
                break;
            case InGameDataType.Wave:
                _currentWave += (int)value;
                break;
        }
    }
    
    
    public void EarnGold(int gold)
    {
        _gold += gold;
    }

    public bool Purchasable(int gold)
    {
        return _gold >= gold;
    }
    
    public bool UseGold(int gold) 
    {
        if (_gold >= gold)
        {
            _gold -= gold;
            return true;
        }
        
        return false;
    }
    
    public async UniTask EarnGoldContinuously() // 주기적으로 획득하는 골드
    {
        while (_playing)
        {
            await UniTask.WaitForSeconds(_interval);
            _gold += _goldPerInterval;
        }
    }
    
    private async UniTask CheckPlayTime()
    {
        while (_playing)
        {
            _chapterPlayTime += Time.deltaTime;
            UniTask.Yield();
        }
    }
}


public enum InGameDataType
{
    TurretCount,
    KilledEnemy,
    BarrierCount,
    TotalHP,
    Gold,
    PlayTime,
    PartMaterialCount,
    Wave
}
