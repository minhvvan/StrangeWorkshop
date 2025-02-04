using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    public static EnemyPathfinder instance;
    
    public List<Transform> barrierPoints = new List<Transform>();
    private BarrierController _barrierController;
    
    //밀림 방지 기능 구현중. 미완성
    public List<Collider> enemyInCounter = new List<Collider>();
    public List<Collider> ignoreColliders = new List<Collider>();
    

    private void Awake()
    {
        instance = this;
    }

    private async void Start()
    {
        _barrierController = GameObject.Find("Barrier").GetComponent<BarrierController>();
        List<Barrier> barriers = new List<Barrier>();
        
        await UniTask.WaitUntil(() =>_barrierController.Barriers != null && _barrierController.Barriers.Count > 0);
        
        foreach (var barrier in _barrierController.Barriers)
        {
            //pivot이 공중에 떠있어서 AI가 위쪽이라 인식함. 대응 필요.
            barrierPoints.Add(barrier.transform);
        }
    }

    //타겟 갱신
    public Transform MatchTarget(Transform enemy)
    {
        List<float> targets = new();
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

    public Transform RandomTarget()
    {
        return barrierPoints[Random.Range(0, barrierPoints.Count)];
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
