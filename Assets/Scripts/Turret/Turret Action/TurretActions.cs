using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurretActions
{
    private Turret _turret;

    public TurretActions(Turret turret)
    {
        _turret = turret;
    }

    public void Reload()
    {
        _turret.turretData.currentBulletNum = _turret.turretData.maxBulletNum;
    }

    public void Hold()
    {
        _turret.turretData.isOnCounter = false;
    }

    public void Put()
    {
        _turret.turretData.isOnCounter = true;
    }

    public bool Upgrade()
    {
        if (_turret.turretUpgrade.Upgradable())
        {
            _turret.turretData.isUpgrading = true;
            return true;
        }

        return false;
    }

    public async UniTask Fix()
    {
        float fixProgress = 0f;
        // Todo: 수리 진척도 UI로 표시
        while (_turret.turretData.isCrashed)
        {
            await UniTask.Yield();
            fixProgress += Time.deltaTime;
            
            Debug.Log(fixProgress);
            
            if (fixProgress >= _turret.turretData.fixTime)
            {
                _turret.turretData.isCrashed = false;
            }
        }
    }

    public void Crash()
    {
        _turret.turretData.isCrashed = true;
    }

    public void SetTargetStrategy(ITargetStrategy newStrategy)
    {
        _turret.turretData.targetStrategy = newStrategy;
    }

    public void SetShootingStrategy(ShootingStrategy newStrategy)
    {
        _turret.turretData.shootingStrategy = newStrategy;
    }
    
    public void UpdateRangeEffectSize()
    {
        float size = _turret.turretData.attackRange * 2f;
        _turret.turretData.rangeEff.transform.localScale = new Vector3(size, size, 1f);
    }
    
    public void UpdateTarget()
    {
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(_turret.transform.position, _turret.turretData.attackRange, layerMask);

        // no enemy in range
        if (hitColliders.Length <= 0)
        {
            _turret.turretData.target = null;
            return;
        }
        _turret.turretData.target = _turret.turretData.targetStrategy.SelectTarget(hitColliders, _turret);
    }
}
