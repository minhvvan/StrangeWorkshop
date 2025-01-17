using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class ShootingStrategy
{
    // 포탑에 따라 shooting 방식이나 타겟을 따라 포탑이 움직이는 전략이 달라짐
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
