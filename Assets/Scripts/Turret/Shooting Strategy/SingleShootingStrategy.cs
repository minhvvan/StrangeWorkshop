using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShootingStrategy : ShootingStrategy
{
    public SingleShootingStrategy(Turret turret) : base(turret) { }
    
    protected override void CreateBullet(GameObject target)
    {
        VFXManager.Instance.TriggerVFX(VFXType.MUZZLE, _turret.turretData.muzzleMain);
        GameObject missleGo = GameObject.Instantiate(_turret.turretData.bullet, _turret.turretData.muzzleMain);
        Bullet bullet = missleGo.GetComponent<Bullet>();
        bullet.InitBullet(target.transform, _turret.turretData.finalDamage);
    }

    protected override void RotateTurretHead(GameObject target)
    {
        Transform turretHead = _turret.turretData.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        turretHead.forward = targetDir;
    }
}
