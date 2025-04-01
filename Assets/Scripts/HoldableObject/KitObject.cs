using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class KitObject : HoldableObject
{
    [Header("KitInfo")]
    public KitType kitType;
    private Action<KitObject, int?> _objective;
    private Action<int?> _upgradeKit;
    
    [Header("KitStat")]
    public int kitLevel;
    [SerializeField] private int _kitRemaningCost;
    public int KitRemainingCost
    {
        get => _kitRemaningCost;
        set
        {
            if (value < 0) value = 0;
            
            _kitRemaningCost = value;
            
            if(_kitRemaningCost == 0) Dispose();
        }
    }
    public float kitValue;
    
    [Header("Option")]
    ///설정하면 cost가 해당 값만큼씩 차감됩니다.
    public int modifyCost = 0;
    
    private async void Awake()
    {
        await SetObjective();
    }
    
    private async UniTask SetObjective() 
    {
        switch (kitType)
        {
            case KitType.REPAIR:
                var repairKitSO = await DataManager.Instance
                    .LoadDataAsync<KitInfoSO>(Addresses.Data.Kits.REPAIR);
                var repairKit = new RepairKit();
                repairKit.SetKitInfo(repairKitSO);
                repairKit.SetKit(this);
                _objective = repairKit.Excute;
                _upgradeKit = repairKit.UpgradeKit;
                break;
            case KitType.NONE:
                Debug.LogError("프리팹에 Type을 설정해주세요");
                Destroy(gameObject);
                break;
        }
    }
    
    /// <summary>
    /// 기본적으로 cost가 1씩 차감됩니다. modifyCost를 설정하면 그 값을 따릅니다.
    /// </summary>
    public void Excute()
    {
        _objective?.Invoke(this, modifyCost != 0 ? modifyCost : null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upgradeValue"></param>
    public void UpgradeKit(int? upgradeValue = null)
    {
        _upgradeKit?.Invoke(upgradeValue);
    }

    public override void InteractAlternate(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out SampleCharacterController player))
        {
            var clearCounter = (ClearCounter)player.GetSelectedInteractableObject();
            clearCounter.RepairCounter(kitValue);
            
            //TODO: 
            
            Excute();
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}