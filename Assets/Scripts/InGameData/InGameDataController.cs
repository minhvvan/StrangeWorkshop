using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class InGameDataController
{
    private CancellationTokenSource _cancellationToken;
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
    private int _interval = 1000;
    private int _goldPerInterval = 1;

    [Header("Time")] 
    private float _chapterPlayTime;
    
    [Header("Crafting")]
    private Dictionary<PartMaterialType, int> _partMaterialCounts;

    [Header("Wave")] 
    private int _currentWave;
    
    private InGameUIController _uiController;
    [Header("Events")]
    public Action<int> OnGoldChanged;
    
    public InGameDataController()
    {
        _turretCounts = new Dictionary<TurretType, int>();
        _killedEnemyCounts = new Dictionary<EnemyType, int>();
        _partMaterialCounts = new Dictionary<PartMaterialType, int>();
        
        var _uiController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _uiController.RegisterGameUI(this);
    }

    public void StartChapter()
    {
        ResetGameData();
        OnGoldChanged?.Invoke(_gold);
        ResumeGame();
    }

    public void ResumeGame()
    {
        _cancellationToken = new CancellationTokenSource();
        _playing = true;
        CheckPlayTime();
        EarnGoldContinuously();
    }

    public void PauseGame()
    {
        _cancellationToken?.Cancel();
        _cancellationToken?.Dispose();
        _playing = false;
    }

    public void ModifyInGameData(InGameDataType dataType, object value, [CanBeNull] object subType = null)
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
        OnGoldChanged?.Invoke(_gold);
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
            OnGoldChanged?.Invoke(_gold);
            return true;
        }
        
        return false;
    }
    
    public async UniTask EarnGoldContinuously() // 주기적으로 획득하는 골드
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            await UniTask.Delay(_interval, cancellationToken:_cancellationToken.Token);
            if (!_cancellationToken.IsCancellationRequested)
            {
                _gold += _goldPerInterval;
                OnGoldChanged?.Invoke(_gold);
            }
        }
    }
    
    private async UniTask CheckPlayTime()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            _chapterPlayTime += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private void ResetGameData()
    {
        // todo: turret 관련 데이터 초기화

        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            _killedEnemyCounts[enemyType] = 0;
        }

        // todo: barrier 관련 데이터 초기화

        _gold = 0;
        _chapterPlayTime = 0f;

        foreach (PartMaterialType partMaterialType in Enum.GetValues(typeof(PartMaterialType)))
        {
            _partMaterialCounts[partMaterialType] = 0;
        }

        _currentWave = 1;
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
