using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory_Guided : Trajectory
{
    // Start is called before the first frame update
    public Trajectory_Guided(Bullet bullet, Vector3 startPos, Transform target) : base(bullet, startPos, target)
    {
    }

    public override void MoveToTarget()
    {
        Vector3 dir = _target.position - _bullet.transform.position;
        //float distThisFrame = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(_bullet.transform.forward, dir, 
            Time.deltaTime * _bullet.bulletData.turnSpeed, 0.0f);
        Debug.DrawRay(_bullet.transform.position, newDirection, Color.red);

        //transform.Translate(dir.normalized * distThisFrame, Space.World);
        //transform.LookAt(target);

        _bullet.transform.Translate(Vector3.forward * Time.deltaTime * 20f);
        _bullet.transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
