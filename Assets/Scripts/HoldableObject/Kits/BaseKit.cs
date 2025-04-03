using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseKit : HoldableObject
{
    protected Action<SampleCharacterController, int?> OnExcute;
    
    [Header("KitInfo"), Tooltip("Kit 종류를 설정합니다."), SerializeField]
    private KitType _kitType; //kit의 종류
    private int _maxLevel; //kit의 최대 레벨
    private int[] _maxCosts; //kit의 레벨별 최대 Cost
    private float[] _kitValues; //kit의 레벨별 능력치
    protected bool IsNoCost {get; set;} //Cost 소진 여부
    
    [Header("KitStat"), Tooltip("KitLevel에 따라 능력치가 다르게 설정됩니다."), SerializeField]
    private int kitLevel; //레벨
    [SerializeField] private int _kitRemaningCost; //남은 Cost
    [SerializeField] protected float kitValue; //능력치
    
    [Header("Option"), Tooltip("선택적 옵션) 설정하면 cost가 해당 값만큼씩 차감됩니다."), SerializeField] 
    private int _modifyCost = 0;
    protected int KitRemainingCost
    {
        get => _kitRemaningCost;
        set
        {
            if (value < 0) value = 0;
            
            _kitRemaningCost = value;

            if (_kitRemaningCost == 0) IsNoCost = true;
        }
    }
    
    /// <summary>
    /// Kit의 SO데이터를 할당합니다.
    /// </summary>
    /// <param name="kitInfo">Kit의 Type에 맞는 SO를 할당 하세요.</param>
    protected void SetKitInfo(KitInfoSO kitInfo)
    {
        _kitType = kitInfo.kitType;
        _maxLevel = kitInfo.maxLevel;
        _maxCosts = kitInfo.maxCosts;
        _kitValues = kitInfo.kitValues;
        _modifyCost = kitInfo.modifyCost;
    }
    
    /// <summary>
    /// Kit의 세부 값을 설정합니다.
    /// </summary>
    /// <param name="upgradeValue">업그레이드를 위해 호출 시 입력 하세요.</param>
    protected void SetKit(int? upgradeValue = null)
    {
        upgradeValue = upgradeValue ?? 0;
        
        var level = 
            (kitLevel = 
                (upgradeValue == 0 ? 1 : Mathf.Clamp(kitLevel + upgradeValue.Value, 1, _maxLevel))
            ) - 1;
        
        KitRemainingCost = _maxCosts[level];
        kitValue = _kitValues[level];
    }
    
    /// <summary>
    /// 업그레이드, 인자없이 호출하면 1단계씩 업그레이드 됩니다.
    /// </summary>
    /// <param name="upgradeValue"></param>
    public void UpgradeKit(int? upgradeValue = null)
    {
        upgradeValue = upgradeValue ?? 1;

        if ((kitLevel + upgradeValue.Value) > _maxLevel)
        {
            Debug.LogError("UpgradeKit: 이미 최대 레벨입니다.");
            return;
        }
        
        if (upgradeValue == 0)
        {
            Debug.LogError("UpgradeKit: 0이 아닌 값으로 호출해야 합니다.");
            return;
        }
        
        SetKit(upgradeValue);
    }
    
    /// <summary>
    /// Kit 상호작용 액션, 새로운 Kit 추가 시, 상호작용 조건 추가 필요.
    /// </summary>
    /// <param name="agent"></param>
    public override void InteractAlternate(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out SampleCharacterController player))
        {
            //상호작용 조건이 갖춰지지 않았다면 던지기 액션으로 전달
            if (_kitType == KitType.REPAIR && (ClearCounter)player.GetSelectedInteractableObject() == null)
            {
                base.InteractAlternate(agent);
                return;
            }
            
            //동작 수행, 기본적으로 cost가 1씩 차감됩니다. modifyCost를 설정하면 그 값을 따릅니다.
            OnExcute?.Invoke(player, _modifyCost != 0 ? _modifyCost : null);
            
            //Cost 다썼다면 제거
            if(IsNoCost) Dispose(player);
        }
    }

    //Kit 제거
    private static void Dispose(SampleCharacterController player)
    {
        player.ClearHoldableObject();
    }
}