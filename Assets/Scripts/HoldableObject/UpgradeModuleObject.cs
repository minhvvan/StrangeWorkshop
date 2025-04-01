using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModuleObject : HoldableObject
{
    public UpgradeDataSO _upgradeDataSO;
    // public override bool SetHoldableObjectParent(IHoldableObjectParent parent)
    // {
    //     // 현재 HoldableObject가 완성품이면 옮길 수 있는 상태인지 검사(Player가 장갑을 꼈는지)
    //     if (!parent.CanSetHoldableObject())
    //     {
    //         return false;
    //     }
    //     
    //     return base.SetHoldableObjectParent(parent);
    // }

    public void SetUpgradeDataSO(UpgradeDataSO upgradeDataSO)
    {
        _upgradeDataSO = upgradeDataSO;
    }

    public UpgradeDataSO GetUpgradeDataSO()
    {
        return _upgradeDataSO;
    }
}
