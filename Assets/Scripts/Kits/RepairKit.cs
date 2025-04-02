using System.Drawing;
using UnityEngine;

public class RepairKit : IInteractKits
{
    private int _maxLevel;
    private int[] _maxCosts;
    private float[] _kitValues;
    private int _modifyCost;
    private KitObject _kitObject;

    public void SetKitInfo(KitInfoSO kitInfo)
    {
        _maxLevel = kitInfo.maxLevel;
        _maxCosts = kitInfo.maxCosts;
        _kitValues = kitInfo.kitValues;
        _modifyCost = kitInfo.modifyCost;
    }
    public void SetKit(KitObject kit)
    {
        _kitObject = kit;
        var level = (_kitObject.kitLevel = 1) - 1;
        _kitObject.KitRemainingCost = _maxCosts[level];
        _kitObject.kitValue = _kitValues[level];
    }

    public void UpgradeKit(int? upgradeValue = null)
    {
        upgradeValue = upgradeValue ?? 1;

        if ((_kitObject.kitLevel + upgradeValue.Value) > _maxLevel)
        {
            Debug.LogError("UpgradeKit: 이미 최대 레벨입니다.");
            return;
        }
        
        if (upgradeValue == 0)
        {
            Debug.LogError("UpgradeKit: 0이 아닌 값으로 호출해야 합니다.");
            return;
        }
        
        var level = (_kitObject.kitLevel += upgradeValue.Value) - 1;
        _kitObject.KitRemainingCost = _maxCosts[level];
        _kitObject.kitValue = _kitValues[level];
    }
    
    public void Excute(KitObject obj, SampleCharacterController player = null, int? cost = null)
    {
        cost = cost ?? 1;
        var clearCounter = (ClearCounter)player.GetSelectedInteractableObject();
        
        //cost 체크
        if (obj.KitRemainingCost - cost.Value < 0)
        {
            Debug.LogError("REPAIR: COST가 부족합니다.");
            return;
        }
        
        //수리동작 수행
        if (!clearCounter.RepairCounter(_kitObject.kitValue))
        {
            Debug.LogError("REPAIR: 이미 내구도가 최대 입니다!");
            return;
        }
        
        //연산처리
        obj.KitRemainingCost -= cost.Value;
        
        //VFX
        VFXManager.Instance.TriggerVFX(VFXType.COUNTERREPAIR, 
            clearCounter.transform.position + new Vector3(0,2f,0));
    }
}