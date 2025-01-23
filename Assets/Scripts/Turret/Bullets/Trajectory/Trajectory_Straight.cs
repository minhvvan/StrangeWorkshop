using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory_Straight : Trajectory
{
    private float _speed;
    public Trajectory_Straight(Bullet bullet, Vector3 startPos, Vector3 endPos) : base(bullet, startPos, endPos)
    {
        InitBullet();
    }
    
    protected void InitBullet()
    {
        Vector3 direction = _endPos - _startPos;
        float distance = (direction).magnitude;
        
        // bullet이 target을 향하도록
        _bullet.transform.rotation = Quaternion.LookRotation(direction);
        _speed = distance / _bullet.bulletData.timeToTarget;
    }

    public override void MoveToTarget()
    {
        float singleSpeed = _speed * Time.deltaTime;
        _bullet.transform.Translate(_bullet.transform.forward * (singleSpeed * 2), Space.World);
    }
}
