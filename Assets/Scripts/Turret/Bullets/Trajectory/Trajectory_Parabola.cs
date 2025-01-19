using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory_Parabola : Trajectory
{
    private Vector3 _velocity;
    private bool _isShot = false;
    
    public Trajectory_Parabola(ProjectileCustom projectile, Vector3 startPoint, Vector3 endPoint) : base(projectile, startPoint, endPoint)
    {
        CalculateVelocity();
    }

    private  void CalculateVelocity()
    {
        float timeToTarget = _projectile.projectileData.timeToTarget;
        Vector3 distance = _endPos - _startPos;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / timeToTarget;
        float Vy = Sy / timeToTarget + 0.5f * Mathf.Abs(Physics.gravity.y) * timeToTarget;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        _velocity = result;
    }
    
    public override void MoveToTarget()
    {
        if (!_isShot)
        {
            _isShot = true;
            _projectile.GetComponent<Rigidbody>().velocity = _velocity;
        }
    }
}
