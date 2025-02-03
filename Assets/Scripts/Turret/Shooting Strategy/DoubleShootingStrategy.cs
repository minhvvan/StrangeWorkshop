using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleShootingStrategy : ShootingStrategy
{
    private bool _shootLeft = true;
    
    public DoubleShootingStrategy(Turret turret) : base(turret) { }

    protected override void CreateBullet(GameObject target)
    {

        Transform muzzleMain = _turret.turretData.muzzleMain;
        GameObject.Instantiate(_turret.turretData.muzzleEff, muzzleMain.transform.position, muzzleMain.rotation);
        GameObject missleGo1 = GameObject.Instantiate(_turret.turretData.bullet, muzzleMain.transform.position, muzzleMain.rotation);
        Bullet bullet1 = missleGo1.GetComponent<Bullet>();
        bullet1.InitBullet(target.transform, _turret.turretData.damage);

        Transform muzzleSub = _turret.turretData.muzzleSub;
        GameObject.Instantiate(_turret.turretData.muzzleEff, muzzleSub.transform.position, muzzleSub.rotation);
        GameObject missleGo2 = GameObject.Instantiate(_turret.turretData.bullet, muzzleSub.transform.position, muzzleSub.rotation);
        Bullet bullet2 = missleGo2.GetComponent<Bullet>();
        bullet2.InitBullet(target.transform, _turret.turretData.damage);
        
    }

    protected override void RotateTurretHead(GameObject target)
    {
        Transform turretHead = _turret.turretData.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        turretHead.forward = targetDir;
        targetDir.y = 0;
        turretHead.transform.rotation = Quaternion.RotateTowards(turretHead.rotation, Quaternion.LookRotation(targetDir), 
            _turret.turretData.lookSpeed * Time.deltaTime);

    }
}
