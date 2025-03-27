using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TurretActions
{
    // private Turret _turret;

    // public TurretActions(Turret turret)
    // {
    //     _turret = turret;
    // }

    // public void Reload()
    // {
    //     _turret.turretData.currentBulletNum = _turret.turretData.maxBulletNum;
    // }

    public static void Hold(Turret turret)
    {
        // _turret.turretData.isOnCounter = false;
        turret.turretData.parentClearCounter = null;
    }

    public static void Put(Turret turret, IHoldableObjectParent parent)
    {
        // _turret.turretData.isOnCounter = true;
        if (parent is ClearCounter)
        {
            turret.turretData.parentClearCounter = (ClearCounter)parent;
        }
    }

    public static bool Upgrade(Turret turret)
    {
        if (turret.turretUpgrade.Upgradable())
        {
            turret.turretUpgrade.ActivateUpgradeBar();
            turret.turretData.isUpgrading = true;
            return true;
        }

        return false;
    }

    // public async UniTask Fix()
    // {
    //     if (!_turret.turretData.isCrashed) return;
    //     
    //     ProgressBar progressBar = _turret.turretData.progressBarFix;
    //     progressBar.gameObject.SetActive(true);
    //     float fixProgress = 0f;
    //     GameObject fixingEff = VFXManager.Instance.TriggerVFX(VFXType.TURRETFIX, _turret.gameObject.transform, returnAutomatically: false);
    //     
    //     // Todo: 수리 진척도 UI로 표시
    //     while (_turret.turretData.isCrashed)
    //     {
    //         await UniTask.Yield();
    //         fixProgress += Time.deltaTime;
    //         progressBar.UpdateProgressBar(fixProgress);
    //         if (fixProgress >= _turret.turretData.fixTime)
    //         {
    //             _turret.turretData.isCrashed = false;
    //             progressBar.ResetBar();
    //             progressBar.gameObject.SetActive(false);
    //             VFXManager.Instance.ReturnVFX(VFXType.TURRETFIX, fixingEff);
    //         }
    //     }
    // }
    //
    // public void Crash()
    // {
    //     _turret.turretData.isCrashed = true;
    // }

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
        float size = turret.turretData.attackRange * 2f;
        turret.turretData.rangeEff.transform.localScale = new Vector3(size, size, 1f);
    }
    
    // public static async UniTask UpdateTarget(Turret turret)
    // {
    //     while (turret.turretData.target == null)
    //     {
    //         int layerMask = LayerMask.GetMask("Enemy");
    //         Collider[] hitColliders = Physics.OverlapSphere(turret.transform.position, turret.turretData.attackRange, layerMask);
    //
    //         // no enemy in range
    //         if (hitColliders.Length <= 0)
    //         {
    //             turret.turretData.target = null;
    //         }
    //         else
    //         {
    //             turret.turretData.target = turret.turretData.targetStrategy.SelectTarget(hitColliders, turret);
    //         }
    //         
    //         await UniTask.Yield();
    //     }
    // }

    public static void UpdateTarget(Turret turret)
    {
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(turret.transform.position, turret.turretData.attackRange, layerMask);
        
        // no enemy in range
        if (hitColliders.Length <= 0)
        {
            turret.turretData.target = null;
            return;
        }
        turret.turretData.target = turret.turretData.targetStrategy.SelectTarget(hitColliders, turret);
    }
}
