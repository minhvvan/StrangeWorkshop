using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShootingStrategy : ShootingStrategy
{
    public SingleShootingStrategy(Turret turret) : base(turret) { }
    
    protected override void CreateBullet(GameObject target)
    {
        GameObject.Instantiate(_turret.turretData.muzzleEff, _turret.turretData.muzzleMain);
        GameObject missleGo = GameObject.Instantiate(_turret.turretData.bullet, _turret.turretData.muzzleMain);
        ProjectileCustom projectile = missleGo.GetComponent<ProjectileCustom>();
        projectile.InitProjectile(target.transform, _turret.turretData.damage);
    }

    protected override void RotateTurretHead(GameObject target)
    {
        Transform turretHead = _turret.turretData.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        turretHead.forward = targetDir;
    }
}
