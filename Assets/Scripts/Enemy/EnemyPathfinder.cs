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
    
    //공개 저장형 백업리스트
    public List<Transform> pbBarrierPoints = new List<Transform>();
    
    // //밀림 방지 기능 구현중. 미완성
    // public List<Collider> enemyInCounter = new List<Collider>();
    // public List<Collider> ignoreColliders = new List<Collider>();
    
    //
    public bool isBPloaded = false;

    private void Awake()
    {
        instance = this;
    }

    private async void Start()
    {
        await Initialize();
    }

    private async UniTask Initialize()
    {
        _barrierController = GameObject.Find("Barrier").GetComponent<BarrierController>();
        
        await UniTask.WaitUntil(() =>_barrierController.Barriers != null && _barrierController.Barriers.Count > 0);
        foreach (var barrier in _barrierController.Barriers)
        {
            barrierPoints.Add(barrier.transform);
        }
        
        await UniTask.WaitUntil(()=>barrierPoints != null);
        pbBarrierPoints.AddRange(barrierPoints);
        
        await UniTask.WaitUntil(()=> pbBarrierPoints != null);
        isBPloaded = true;
    }

   /// <summary>
   /// 가장 가까운 방벽위치를 찾습니다.
   /// </summary>
   /// <param name="order">본인의 Transform을 받습니다.</param>
   /// <returns>가장 가까운 타겟을 반환합니다.</returns>
    public Transform MatchTarget(Transform order)
    {
        List<float> targets = new();
        float nearTarget = 0f;
        
        foreach (Transform barrier in barrierPoints)
        {
            targets.Add(Vector3.Distance(order.position, barrier.position));
        }

        //가장 거리가 가까운 것 찾기.
        int bind = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            //최초 1회 일단 할당, 이후 값이 작을수록 재할당.
            if (nearTarget == 0 || nearTarget > targets[i])
            {
                nearTarget = targets[i];
                bind = i;
            }
        }

        return barrierPoints[bind];
    }
    
    /// <summary>
    /// 지정한 대상을 제외하고 탐색합니다.
    /// </summary>
    /// <param name="order">본인의 Transform을 받습니다.</param>
    /// <param name="disableTarget">탐색을 제외할 타겟의 Transform을 받습니다.</param>
    /// <returns>지정한 대상을 제외하고 가장 가까운 타겟을 반환합니다.</returns>
    public Transform ExcludeMatchTarget(Transform order, Transform disableTarget)
    {
        //Debug.Log("Exclude Match Target");
        List<Transform> targets = new();
        float nearTarget = Mathf.Infinity;
        
        foreach (Transform barrier in barrierPoints)
        {
            //지정대상을 제외하고 할당함.
            if (barrier != disableTarget)
            {
                //targets.Add(Vector3.Distance(order.position, barrier.position));
                targets.Add(barrier);
            }
        }

        //가장 거리가 가까운 것 찾기.
        int bind = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = Vector3.Distance(order.position, targets[i].position);
            //값이 작을수록 재할당.
            if (nearTarget > distance)
            {
                nearTarget = distance;
                bind = i;
            }
        }

        return targets[bind];
    }

    /// <summary>
    /// 타겟을 리스트에서 제거합니다.
    /// </summary>
    /// <param name="target">방벽위치 리스트에서 제외할 타겟을 받습니다.</param>
    public void RemoveTarget(Transform target)
    {
        if (barrierPoints.Contains(target))
        {
            //Debug.Log("RemoveTarget" + target.name);
            barrierPoints.Remove(target);
        }
        else
        {
            Debug.Log("Target not match. cannot remove barrier.");
        }
    }

    ///아무 타겟이나 랜덤하게 받습니다.
    public Transform RandomTarget()
    {
        return barrierPoints[Random.Range(0, barrierPoints.Count)];
    }

    // //충돌 상태 전체갱신
    // public void ColliderControl()
    // {
    //     if (ignoreColliders != null && enemyInCounter != null)
    //     {
    //         for (int i = 0; i < ignoreColliders.Count; i++)
    //         {
    //             foreach (var enemys in enemyInCounter)
    //             {
    //                 Physics.IgnoreCollision(
    //                     ignoreColliders[i], enemys, true);
    //             }
    //         }
    //     }
    // }
    //
    // //신규 생성 적 충돌갱신
    // public void ColliderSet(Collider targetCol)
    // {
    //     if (ignoreColliders != null)
    //     {
    //         foreach (var cols in ignoreColliders)
    //         {
    //             Physics.IgnoreCollision(
    //                 cols, targetCol, true);
    //         }
    //     }
    // }
    //
    // //충돌 비활성화
    // public void ColliderDisable(Collider targetCol)
    // {
    //     ignoreColliders.Add(targetCol);
    //     foreach (var enemys in enemyInCounter)
    //     {
    //         Physics.IgnoreCollision(
    //             targetCol, enemys, true);
    //     }
    // }
    //
    // //충돌 재활성화
    // public void ColliderReEnable(Collider targetCol)
    // {
    //     foreach (var enemys in enemyInCounter)
    //     {
    //         Physics.IgnoreCollision(
    //             targetCol, enemys, false);
    //     }
    //     ignoreColliders.Remove(targetCol);
    // }

    
}
