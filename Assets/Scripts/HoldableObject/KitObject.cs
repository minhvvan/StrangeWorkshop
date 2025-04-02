using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class KitObject : HoldableObject
{
    [Header("KitInfo"), Tooltip("*에디터 할당 필수* Kit 종류를 설정합니다.")]
    public KitType kitType;
    
    //
    private Action<KitObject, SampleCharacterController, int?> _objective;
    private Action<int?> _upgradeKit;
    
    [Header("KitStat"), Tooltip("KitLevel에 따라 능력치가 다르게 설정됩니다.")]
    public int kitLevel;
    
    [SerializeField] private int _kitRemaningCost;
    
    /// <summary>
    ///해당 Kit에 남은 Cost, 0이 되면 Kit을 제거합니다.
    /// </summary>
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
    
    /// <summary>
    /// Kit 사용 시 전달 될 값
    /// </summary>
    public float kitValue;
    
    [Header("Option"), Tooltip("선택적 옵션) 설정하면 cost가 해당 값만큼씩 차감됩니다.")] 
    public int modifyCost = 0;
    
    private async void Awake()
    {
        await SetObjective();
    }
    
    /// <summary>
    /// KitObject의 KitType에 따라 다른 로직이 할당 됩니다.
    /// </summary>
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
    public void Excute(SampleCharacterController player)
    {
        _objective?.Invoke(this, player, modifyCost != 0 ? modifyCost : null);
    }

    /// <summary>
    /// 업그레이드, 인자없이 호출하면 1단계씩 업그레이드 됩니다.
    /// </summary>
    /// <param name="upgradeValue"></param>
    public void UpgradeKit(int? upgradeValue = null)
    {
        _upgradeKit?.Invoke(upgradeValue);
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
            if (kitType == KitType.REPAIR && (ClearCounter)player.GetSelectedInteractableObject() == null)
            {
                base.InteractAlternate(agent);
                return;
            }
            
            //동작 수행
            Excute(player);
        }
    }

    //
    public void Dispose()
    {
        Destroy(gameObject);
    }
}