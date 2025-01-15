using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShootingStrategy : ShootingStrategy
{
    public SingleShootingStrategy(Turret turret) : base(turret) { }
    
    protected override void CreateBullet(GameObject target)
    {
        Object.Instantiate(_turret.turretData.muzzleEff, _turret.turretData.muzzleMain);
        GameObject missleGo = Object.Instantiate(_turret.turretData.bullet, _turret.turretData.muzzleMain);
        Projectile projectile = missleGo.GetComponent<Projectile>();
        projectile.target = target.transform;
    }

    protected override void RotateTurretHead(GameObject target)
    {
        Transform turretHead = _turret.turretData.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        turretHead.forward = targetDir;
    }
}
