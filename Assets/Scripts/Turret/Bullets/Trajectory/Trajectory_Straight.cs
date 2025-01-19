using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory_Straight : Trajectory
{
    private float _speed;
    public Trajectory_Straight(ProjectileCustom projectile, Vector3 startPos, Vector3 endPos) : base(projectile, startPos, endPos)
    {
        InitBullet();
    }
    
    protected void InitBullet()
    {
        Vector3 direction = _endPos - _startPos;
        float distance = (direction).magnitude;
        
        // bullet이 target을 향하도록
        _projectile.transform.rotation = Quaternion.LookRotation(direction);
        _speed = distance / _projectile.projectileData.timeToTarget;
    }

    public override void MoveToTarget()
    {
        float singleSpeed = _speed * Time.deltaTime;
        _projectile.transform.Translate(_projectile.transform.forward * (singleSpeed * 2), Space.World);
    }
}
