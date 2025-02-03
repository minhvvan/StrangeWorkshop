using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    public static EnemyPathfinder instance;
    
    public List<Transform> barrierPoints = new List<Transform>();
    public List<Collider> enemyInCounter = new List<Collider>();
    public List<Collider> ignoreColliders = new List<Collider>();

    void Awake()
    {
        instance = this;
    }
    
    //타겟 갱신
    public Transform MatchTarget(Transform enemy)
    {
        List<float> targets = new List<float>();
        float nearTarget = 0f;
        
        foreach (Transform barrier in barrierPoints)
        {
            targets.Add(Vector3.Distance(enemy.position, barrier.position));
        }

        //가장 거리가 가까운 것 찾기.
        int bind = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            //최초 1회 일단 할당
            if (nearTarget == 0)
            {
                nearTarget = targets[i];
                bind = i;
            }
            
            //이후 값이 작을수록 재할당.
            if (nearTarget > targets[i])
            {
                nearTarget = targets[i];
                bind = i;
            }
        }

        return barrierPoints[bind];
    }

    //충돌 상태 전체갱신
    public void ColliderControl()
    {
        if (ignoreColliders != null && enemyInCounter != null)
        {
            for (int i = 0; i < ignoreColliders.Count; i++)
            {
                foreach (var enemys in enemyInCounter)
                {
                    Physics.IgnoreCollision(
                        ignoreColliders[i], enemys, true);
                }
            }
        }
    }

    //신규 생성 적 충돌갱신
    public void ColliderSet(Collider targetCol)
    {
        if (ignoreColliders != null)
        {
            foreach (var cols in ignoreColliders)
            {
                Physics.IgnoreCollision(
                    cols, targetCol, true);
            }
        }
    }

    //충돌 비활성화
    public void ColliderDisable(Collider targetCol)
    {
        ignoreColliders.Add(targetCol);
        foreach (var enemys in enemyInCounter)
        {
            Physics.IgnoreCollision(
                targetCol, enemys, true);
        }
    }

    //충돌 재활성화
    public void ColliderReEnable(Collider targetCol)
    {
        foreach (var enemys in enemyInCounter)
        {
            Physics.IgnoreCollision(
                targetCol, enemys, false);
        }
        ignoreColliders.Remove(targetCol);
    }

    
}
