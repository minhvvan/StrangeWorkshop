using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestTargetStrategy : ITargetStrategy
{
    // 가장 가까운 object를 반환한다.
    public GameObject SelectTarget(Collider[] targets, GameObject turret)
    {
        GameObject closestTarget = null;
        float closestDist = Mathf.Infinity;
        
        foreach (Collider collider in targets)
        {
            float dist = Vector3.Distance(turret.transform.position, collider.transform.position);

            if (dist < closestDist)
            {
                closestTarget = collider.gameObject;
                closestDist = dist;
            }
        }
        
        return closestTarget;
    }
}
