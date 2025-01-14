using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class ShootingStrategy
{
    protected Turret _turret;

    public ShootingStrategy(Turret turret)
    {
        _turret = turret;
    }

    public void Shoot(GameObject target)
    {
        CreateBullet(target);
        _turret.remainingBulletsNum--;
    }

    public void FollowTarget(GameObject target)
    {
        RotateTurretHead(target);
    }
    
    protected abstract void CreateBullet(GameObject target);
    protected abstract void RotateTurretHead(GameObject target);
}
