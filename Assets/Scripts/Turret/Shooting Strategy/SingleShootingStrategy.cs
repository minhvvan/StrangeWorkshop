using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShootingStrategy : ShootingStrategy
{
    public SingleShootingStrategy(Turret turret) : base(turret) { }
    
    protected override void CreateBullet(GameObject target)
    {
        Object.Instantiate(_turret.turret.muzzleEff, _turret.turret.muzzleMain);
        GameObject missleGo = Object.Instantiate(_turret.turret.bullet, _turret.turret.muzzleMain);
        Projectile projectile = missleGo.GetComponent<Projectile>();
        projectile.target = target.transform;
    }

    protected override void RotateTurretHead(GameObject target)
    {
        Transform turretHead = _turret.turret.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        turretHead.forward = targetDir;
    }
}
