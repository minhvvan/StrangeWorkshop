using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory_Straight : Trajectory
{
    private float _speed;
    private Vector3 _endPos;
    public Trajectory_Straight(Bullet bullet, Vector3 startPos, Transform target) : base(bullet, startPos, target)
    {
        InitBullet();
    }
    
    protected void InitBullet()
    {
        _endPos = _target.position;
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
