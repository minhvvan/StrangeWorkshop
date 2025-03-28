using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.Timeline;

public interface IBlackboardEnemy
{
    void InitBlackboard();
}

//블랙보드 담당 클래스
public class BlackboardEnemy : MonoBehaviour, IBlackboardEnemy
{
    [NonSerialized] public Animator anim;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public CapsuleCollider capsuleCol;
    public NavMeshAgent agent;
    public EnemyStatus enemyStatus;
    public Transform target;
    public Transform disableTarget;
    public RaycastHit hit;
    public Collider targetCollider;
    

    //비동기를 도중에 간섭할 수 있는 클래스.
    public CancellationTokenSource cts;
    public CancellationTokenSource autoResearchCts;

    ///킬 스위치, false가 되면 모든 동작이 멈춘다.
    public bool bEnable = true;
    private BossEndEventSO _bossEndEventSO;
    
    ///방벽감지 및 공격
    private IAttackPattern _atkPattern;
    private bool _bDetectedFromTower;
    
    /// <summary>
    /// 은신 감지타워의 범위내에 들면 호출해주세요.
    /// </summary>
    public bool DetectedFromTower
    {
        get => _bDetectedFromTower;
        set
        {
            _bDetectedFromTower = value;
            
            //true면 공격받도록 Layer변경
            if (_bDetectedFromTower)
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                ChangeMatColor(matObject, enemyStatus.hp, 1f);
            }
            //false면 공격받지 않도록 Layer변경
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                ChangeMatColor(matObject, enemyStatus.hp, 0.4f);
            }
        }
    }

    [NonSerialized] public string layerName = "Barrier";
    public bool bDetectBarrier = false;
    public bool bCanPattern = false;

    public SampleCharacterController player;
    
    ///자동 재검색 시간. 최초 시간만 20초.
    private float _researchTime = 10f;
    public bool researchOrder = false;
    public bool useAutoResearch = true;

    public bool priorityIncrease = false;
    public int priorityStack = 0;
    
    private static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Attack = Animator.StringToHash("Attack");
    
    //Material 오브젝트 할당용
    public Transform matObject;

    //이 녀석이 보스인가? 판별용
    public enum IsBoss
    {
        NONE,
        BOSS
    };
    
    public IsBoss thisBoss = IsBoss.NONE;

    public int currentState = 0;
    
    public void InitBlackboard()
    {
        //비동기 함수 Delay 즉시종료용
        cts = new CancellationTokenSource();
            
        anim = GetComponent<Animator>();
        //부딫혔다고 빙빙 돌지않게.
        rb = GetComponent<Rigidbody>();
        rb.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        
        
        capsuleCol = GetComponent<CapsuleCollider>();

        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = 2;
         
        //Hierarchy에서 첫번째 자식(matertial이 적용된 자식)에게 접근
        foreach (Transform child in transform)
        {
            matObject = child;
            break;
        }
        
        //자동 재검색 타이머 시작.
        AutoResearchTarget().Forget();
        PriorityIncreaser().Forget();
        //최초 이후 재검색 대상시간은 10초.
        _researchTime = 5f;
    }
    private async UniTask PriorityIncreaser()
    {
        agent.avoidancePriority = priorityStack + 7;
        //TODO: cts 추가할 것.
        await UniTask.Delay(1000);
        await UniTask.WaitUntil(() => priorityIncrease);
        PriorityIncreaser().Forget();
    }
    
    public void SetModifyStat()
    {
        enemyStatus.maxHp = enemyStatus.hp;
    }

    public async UniTask SetTypeSetting()
    {
        switch (enemyStatus.enemytype)
        {
            case EnemyType.MeleeBruiser:
                //보스 테스트를 위한 임시 할당.
                thisBoss = IsBoss.BOSS;
                _bossEndEventSO = await DataManager.Instance.LoadDataAsync<BossEndEventSO>
                    (Addresses.Events.Game.BOSS_END);
                break;
            case EnemyType.MeleeFlanker:
                useAutoResearch = false;
                break;
            case EnemyType.MeleeHider:
                DetectedFromTower = false;
                break;
            case EnemyType.RangeMage:
                player = FindObjectOfType<SampleCharacterController>();
                break;
            case EnemyType.Chapter1Boss:
                player = FindObjectOfType<SampleCharacterController>();
                thisBoss = IsBoss.BOSS;
                _bossEndEventSO = await DataManager.Instance.LoadDataAsync<BossEndEventSO>
                    (Addresses.Events.Game.BOSS_END);
                break;
        }
    }
    
    ///적 공격종류 선택
    public void SetPattern()
    {
        _atkPattern = PatternHandler.CreatePattern(enemyStatus.enemytype);
        _atkPattern.InitPattern(this);
    }
    
    public void DestroyPattern()
    {
        _atkPattern.ClearPattern();
        _atkPattern = null;
    }
    
    ///패턴 수행
    public async UniTask OnAttack()
    {
        //타겟의 체력이 0이면
        // if (targetCollider != null)
        // {
        //     if(target.GetComponent<Barrier>().BarrierStat.health <= 0)
        //     {
        //         //체력이 0인 타겟을 리스트에서 방출.
        //         EnemyPathfinder.instance.RemoveTarget(target);
        //         targetCollider = null;
        //         target = null;
        //         bCanPattern = false;
        //         ResearchTarget();
        //         return;
        //     }
        // }
        
        //공격 판정 동작.
        _atkPattern?.RunPattern();
        await UniTask.WaitUntil(() => !bCanPattern);
    }
    
    //Material 색상변환 함수.
    public void ChangeMatColor(Transform child, float hp, float alpha = 1f)
    {
        if (child != null)
        {
            //현재 체력을 최대 체력에 비례해여 0~1로 반환.
            float colorValue = Mathf.InverseLerp(0f, enemyStatus.maxHp, hp);
            
            Color nextColor = new Color(colorValue, colorValue, colorValue, alpha);
            
            //색상 변화
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            { 
                childRenderer.material.SetColor("_BaseColor", nextColor);
            }
        }
    }
    
    //따라갈 타겟 지정
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
    
    /// <summary>
    /// 일정시간이 지나도 타겟에게 공격을 해내지 못하면 타겟을 재검색 합니다.
    /// </summary>
    public async UniTask AutoResearchTarget()
    {
        if (!useAutoResearch) return;
         
        researchOrder = true;
        autoResearchCts = new CancellationTokenSource();
        await UniTask.WaitForSeconds(_researchTime,
            cancellationToken: autoResearchCts.Token);
    
        if (bDetectBarrier)
        {
            researchOrder = false;
            autoResearchCts?.Cancel();
            return;
        }
        ExcludeResearchTarget();
    
        researchOrder = false;
        autoResearchCts?.Cancel();
    }
    
    //길찾기 기능//

    ///target위치로 경로를 설정합니다.
    public void SetPathfinder()
    {
        if (target != null)
        {
            //SetCheckDestination(target.position);
            agent.SetDestination(target.position);
        }
        agent.speed = enemyStatus.moveSpeed;
    }

    ///가까운 배리어를 재탐색합니다.
    public void ResearchTarget()
    {
        SetTarget(EnemyPathfinder.instance.MatchTarget(transform));
        
        //SetCheckDestination(target.position);
        agent.SetDestination(target.position);
    }
    
     //지정 대상을 제외하고 배리어를 재탐색 합니다.
     public void ExcludeResearchTarget()
     {
         disableTarget = target;
         SetTarget(EnemyPathfinder.instance.
             ExcludeMatchTarget(transform, disableTarget));
         //SetCheckDestination(target.position);
         agent.SetDestination(target.position);
     }

     public void RandomResearchTarget()
     {
         SetTarget(EnemyPathfinder.instance.RandomTarget());
         agent.SetDestination(target.position);
     }

     ///길찾기를 멈춥니다.
     public void StopTracking()
     {
         agent.isStopped = true;
         //agent.updatePosition = false;
         //agent.updateRotation = false;
     }

     ///길찾기를 재개합니다.
     public void ResumeTracking()
     {
         agent.isStopped = false;
         //agent.updatePosition = true;
         //agent.updateRotation = true;
     }
     
    ///공격 범위 내 적 찾기
    public void SearchNearTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position + new Vector3(0,1f,0), 
            enemyStatus.attackRange, 
            1 << LayerMask.NameToLayer(layerName));
        
        //대상을 범위내에서 찾지 못했을 때
         if (hitColliders.Length == 0)
         {
             Debug.Log("Cannot find target!");
             targetCollider = null;
             return;
         }

         //공격 범위 내 가장 가까운 오브젝트 찾기.
         foreach (Collider c in hitColliders)
         {
             //타겟과 같은 경우에만 공격대상으로 할당.
             if (c.transform == target)
             {
                 targetCollider = c;
             }
         }
         //공격 범위 내 가장 가까운 오브젝트 찾기.
        //float distance = 0f;
        // foreach (Collider c in hitColliders)
        // {
        //     // //최초 초기화
        //     // if (distance == 0 || 
        //     //     distance > Vector3.Distance(transform.position, c.transform.position))
        //     // {
        //     //     distance = Vector3.Distance(transform.position, c.transform.position);
        //     //     
        //     //     //체력이 0이 아닐때만 타겟으로 삼는다.
        //     //     if (c.GetComponent<Barrier>().BarrierStat.health > 0)
        //     //     {
        //     //         targetCollider = c;
        //     //     }
        //     // }
        // }
        
        //
        // if (targetCollider == null)
        // {
        //     //주변 타겟 재검색
        //     ResearchTarget();
        // }
    }

    public void AnimSetSpeed(float speed)
    {
        anim.speed = speed;
    }

    ///CrossFade 간소화.
    public void AnimCrossFade(string animName,float normalizedTime = 0f)
    {
        //anim.SetFloat(Speed, 0.0f);
        anim.CrossFade(animName, 0, 0, normalizedTime);
    }
    
    ///Idle 모션
    public void AnimIdle()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Idle"))
        {
            AnimCrossFade("Idle");
        }
        anim.SetFloat(Speed, 0.0f);
    }

    ///Walk 모션
    public void AnimWalk()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Idle"))
        {
            AnimCrossFade("Idle");
        }
        anim.SetFloat(Speed, 1.0f);
    }

    ///Attack 모션
    public void AnimAttack()
    {
        //걷기 중단.
        //anim.SetFloat(Speed, 0.0f);

        //좌우 번갈아가며 공격하는 모션
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("AttackL") && !stateInfo.IsName("AttackR"))
        {
            AnimCrossFade("AttackL");
            //anim.SetBool(Attack, false);
        }
        else
        {
            if (stateInfo.IsName("AttackL"))
            {
                //anim.SetBool(Attack, true);
                AnimCrossFade("AttackR");
            }
            else if (stateInfo.IsName("AttackR"))
            {
                //anim.SetBool(Attack, false);
                AnimCrossFade("AttackL");
            }
        }
    }
    
    ///Dead 모션
    public void AnimDead()
    {
        AnimCrossFade("Dead");
        
        //킬스위치 활성화
        bEnable = false;
    }
    
    public void OnBossEnd()
    {
        if (_bossEndEventSO != null)
        {
            _bossEndEventSO.Raise();
        }
    }
}
