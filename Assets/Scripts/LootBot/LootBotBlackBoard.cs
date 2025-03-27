using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class LootBotBlackBoard : BaseBlackBoard, IDamageable
{
    #region Components

    public LootBotStats stats = new LootBotStats();
    public SphereCollider goldCollectCollider;

    #endregion

    #region Data
    
    public float startAnimTime = .3f;
    public float stopAnimTime = .15f;
    public float desiredRotationSpeed;
    public float allowRotation;
    public bool canMove = true;

    public float interactionDistance;
    public LayerMask interactLayerMask;
    #endregion

    #region Ref
    public LootBot lootBot;
    public IInteractable _selectedInteractable;
    #endregion
    
    public Action OnBotPowerDown;
    
    private CancellationTokenSource _cts;

    protected override void Awake()
    {
        base.Awake();
        
        //stat에 맞게 초기화
        lootBot = GetComponent<LootBot>();
        goldCollectCollider.radius = stats.CollectionRadius;
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        StartEnergyConsumption(_cts.Token).Forget();
    }
    
    private async UniTask StartEnergyConsumption(CancellationToken cts)
    {
        float time = 0f;
        float duration = stats.EnergyConsumptionDuration;
        float rate = stats.EnergyConsumptionRate;
        
        while (stats.CurrentEnergy > 0 && !cts.IsCancellationRequested)
        {
            time += Time.deltaTime;

            if (time >= duration)
            {
                time = 0f;
                stats.CurrentEnergy -= rate;
            }
            
            await UniTask.Yield();
        }

        if (cts.IsCancellationRequested)
        {
            // Debug.Log("Energy Consumption Cancelled");
        }

        if (stats.CurrentEnergy <= 0)
        {
            OnBotPowerDown?.Invoke();
        }
    }

    public void AddGold(int amount)
    {
        stats.CurrentGold += amount;
        stats.CurrentGold = Mathf.Min(stats.CurrentGold, stats.MaxGoldCapacity);

        //TODO: UI 업데이트??
    }

    public void TakeDamage(float damage)
    {
        stats.CurrentEnergy -= damage;
        //TODO: UI 업데이트 + 데미지 주는 로직에서 Call
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}

[Serializable]
public struct LootBotStats
{
    // 생존 관련 속성
    public float MaxEnergy;
    public float CurrentEnergy;
    public float EnergyConsumptionRate;  // 에너지 소비 양
    public float EnergyConsumptionDuration; // 에너지 소비 주기

    // 수집 능력 관련 속성
    public int MaxGoldCapacity;  // 최대 수집 용량
    public int CurrentGold;  // 현재 수집된 양
    
    // 수집 능력 관련 속성
    public float CollectionRadius;  // 아이템 수집 범위
    
    // 골드 관련 속성
    public float GoldMultiplier;  // 골드 획득 증가율

    // 이동 속도
    public float MoveSpeed;
}