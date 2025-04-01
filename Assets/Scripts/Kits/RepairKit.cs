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
        _kitObject.kitLevel = 1;
        _kitObject.KitRemainingCost = _maxCosts[kit.kitLevel];
        _kitObject.kitValue = _kitValues[kit.kitLevel];
    }

    public void UpgradeKit(int? upgradeValue = null)
    {
        upgradeValue = upgradeValue ?? 1;

        if ((_kitObject.kitLevel + upgradeValue.Value) > _maxLevel)
        {
            Debug.LogError("이미 최대 레벨입니다.");
            return;
        }
        
        if (upgradeValue == 0)
        {
            Debug.LogError("0이 아닌 값으로 호출해야 합니다.");
            return;
        }
        
        var level = _kitObject.kitLevel += upgradeValue.Value;
        _kitObject.KitRemainingCost = _maxCosts[level];
        _kitObject.kitValue = _kitValues[level];
    }
    
    public void Excute(KitObject obj, int? cost = null)
    {
        cost = cost ?? 1;
        obj.KitRemainingCost -= cost.Value;
    }
}