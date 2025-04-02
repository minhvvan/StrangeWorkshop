using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TurretActions
{
    public static void Hold(Turret turret)
    {
        turret.turretData.parentClearCounter = null;
    }

    public static void Put(Turret turret, IHoldableObjectParent parent)
    {
        if (parent is ClearCounter)
        {
            turret.turretData.parentClearCounter = (ClearCounter)parent;
        }
    }

    public static async void Upgrade(Turret turret, UpgradeDataSO upgradeDataSO)
    {
        // if (turret.turretUpgrade.Upgradable())
        // {
        turret.turretUpgrade.ActivateUpgradeBar();
        turret.turretData.isUpgrading = true;
        await turret.turretUpgrade.UpgradeProgressively(upgradeDataSO);
        turret.turretData.isUpgrading = false;
        
    }
    
    public static void SetTargetStrategy(Turret turret, ITargetStrategy newStrategy)
    {
        turret.turretData.targetStrategy = newStrategy;
    }

    public static void SetShootingStrategy(Turret turret, ShootingStrategy newStrategy)
    {
        turret.turretData.shootingStrategy = newStrategy;
    }
    
    public static void UpdateRangeEffectSize(Turret turret)
    {
        float size = turret.turretData.finalAttackRange * 2f;
        turret.turretData.rangeEff.transform.localScale = new Vector3(size, size, 1f);
    }

    public static void UpdateTarget(Turret turret)
    {
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(turret.transform.position, turret.turretData.finalAttackRange, layerMask);
        
        // no enemy in range
        if (hitColliders.Length <= 0)
        {
            turret.turretData.target = null;
            return;
        }
        turret.turretData.target = turret.turretData.targetStrategy.SelectTarget(hitColliders, turret);
    }
}
