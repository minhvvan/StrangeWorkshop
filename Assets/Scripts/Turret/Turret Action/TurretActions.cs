using System.Collections;
using System.Collections.Generic;
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

    public void Upgrade()
    {
        _turret.turretData.isUpgrading = true;
    }
    
    public bool Upgradable()
    {
        return _turret.turretUpgrade.Upgradable();
    }
    
    public void Fix()
    {
        _turret.turretData.currentHealth = _turret.turretData.maxHealth;
    }

    public void Fix(float time)
    {
        _turret.turretData.currentHealth += _turret.turretData.fixSpeed * time;
    }

    public void ReduceHealth()
    {
        _turret.turretData.currentHealth -= 0.1f;
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
