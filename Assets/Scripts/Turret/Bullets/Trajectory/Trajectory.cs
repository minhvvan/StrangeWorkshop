using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trajectory
{
    protected Bullet _bullet;
    protected Vector3 _startPos;
    protected Transform _target;

    public Trajectory(Bullet bullet, Vector3 startPos, Transform target)
    {
        _bullet = bullet;
        _startPos = startPos;
        _target = target;
    }
    // target까지 프레임당 이동
    // projectile의 update에서 호출하는 방식
    public abstract void MoveToTarget();
}
