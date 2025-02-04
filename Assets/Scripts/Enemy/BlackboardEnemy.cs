using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
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
    //public RaycastHit hit;
    public Collider targetCollider;

    //비동기를 도중에 간섭할 수 있는 클래스.
    public CancellationTokenSource cts;
    public CancellationTokenSource rScts;

    //킬 스위치 개념.
    public bool bEnable = true;
    
    //방벽감지 및 공격
    private IAttackPattern _atkPattern;
    
    [NonSerialized] public string layerName = "Barrier";
    public bool bDetectBarrier = false;
    public bool bCanAttack = false;
    public float researchTime = 10f;
    
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    //Material 오브젝트 할당용
    public Transform matObject;
    
    public void InitBlackboard()
    {
        //비동기 함수 Delay 즉시종료용
        cts = new CancellationTokenSource();
        rScts = new CancellationTokenSource();
        
        anim = GetComponent<Animator>();
        //부딫혔다고 빙빙 돌지않게.
        rb = GetComponent<Rigidbody>();
        rb.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        
        
        capsuleCol = GetComponent<CapsuleCollider>();

        agent = GetComponent<NavMeshAgent>();
        
        //Hierarchy에서 첫번째 자식(matertial이 적용된 자식)에게 접근
        foreach (Transform child in transform)
        {
            matObject = child;
            break;
        }
    }
    
    //
    public void SetMaxHp()
    {
        enemyStatus.maxHp = enemyStatus.hp;
    }
    
    //Material 색상변환 함수.
    public void ChangeMatColor(Transform child, float hp)
    {
        if (child != null)
        {
            //현재 체력을 최대 체력에 비례해여 0~1로 반환.
            float colorValue = Mathf.InverseLerp(0f, enemyStatus.maxHp, hp);
            
            Color nextColor = new Color(colorValue, colorValue, colorValue);
            
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
    
    //길찾기 기능//
    
    public void SetPathfinder()
    {
        agent.SetDestination(target.position);
        agent.speed = enemyStatus.moveSpeed;
    }

    public void ResearchTarget()
    {
        SetTarget(EnemyPathfinder.instance.MatchTarget(transform));
        agent.SetDestination(target.position);
    }

    public void StopTracking()
    {
        agent.isStopped = true;
    }

    public void ResumeTracking()
    {
        agent.isStopped = false;
        
    }

    //적 공격종류 선택
    public void SetPattern()
    {
        _atkPattern = PatternHandler.CreatePattern(enemyStatus.enemytype);
    }

    //주기적으로 근처 타겟을 재검색
    public async UniTask RemindSearch(int loop)
    {
        while (loop > 0)
        {
            //킬 스위치 활성화 시, 공격 범위내에 있을 시 종료.
            if (bEnable || bDetectBarrier)
            {
                loop = 0;
            }
            
            await UniTask.Delay((int)(1000 * researchTime),
                cancellationToken: rScts.Token);
            
            SetTarget(EnemyPathfinder.instance.RandomTarget());
            agent.SetDestination(target.position);
        }
    }

    //공격 범위 내 적 찾기
    public void SearchNearTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position + new Vector3(0,1f,0), 
            enemyStatus.attackRange, 
            1 << LayerMask.NameToLayer(layerName));
        
        //대상을 범위내에서 찾지 못했을 때
        if (hitColliders.Length == 0)
        {
            targetCollider = null;
            return;
        }
        
        //공격 범위 내 가장 가까운 오브젝트 찾기.
        float distance = 0f;
        foreach (Collider c in hitColliders)
        {
            //최초 초기화
            if (distance == 0)
            {
                distance = Vector3.Distance(transform.position, c.transform.position);
                targetCollider = c;
            }

            //가까우면 대입
            if (distance > Vector3.Distance(transform.position, c.transform.position))
            {
                distance = Vector3.Distance(transform.position, c.transform.position);
                targetCollider = c;
            }
        }
    }

    public async UniTask OnAttack()
    {
        //판정 적용 선딜레이, 재공격 대기시간.
        float atkFirstDelay = enemyStatus.animFirstDelay;
        float atkCooldown = enemyStatus.attackSpeed;
        
        //공격 모션 시작.
        AnimAttack();
        
        //공격 모션에 맞게 판정이 나가도록 선딜레이 적용
        await UniTask.Delay(
            (int)(1000*atkFirstDelay),
            cancellationToken: cts.Token);
        if (!bCanAttack) return;
        
        //공격 판정 동작.
        _atkPattern?.RunPattern(targetCollider, enemyStatus.attackDamage);
        
        //공격 대기시간
        await UniTask.Delay( 
            //공격 대기시간에 모션 선딜레이시간을 빼서 딜레이 맞추기.
            (int)(1000*(atkCooldown - atkFirstDelay)), 
            //공격 도중 죽으면 도중에 취소해야한다.
            cancellationToken: cts.Token);
        bCanAttack = false;
    }

    //CrossFade 간소화.
    public void AnimCrossFade(string animName)
    {
        anim.CrossFade(animName, 0.1f);
    }
    
    //Idle 모션
    public void AnimIdle()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Idle"))
        {
            AnimCrossFade("Idle");
        }
        anim.SetFloat(Speed, 0.0f);
    }

    //Walk 모션
    public void AnimWalk()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Idle"))
        {
            AnimCrossFade("Idle");
        }
        anim.SetFloat(Speed, 1.0f);
    }

    //Attack 모션
    public void AnimAttack()
    {
        //걷기 중단.
        anim.SetFloat(Speed, 0.0f);

        //좌우 번갈아가며 공격하는 모션
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("AttackL") && !stateInfo.IsName("AttackR"))
        {
            AnimCrossFade("AttackL");
            anim.SetBool(Attack, false);
        }
        else
        {
            if (stateInfo.IsName("AttackL"))
            {
                anim.SetBool(Attack, true);
            }
            else if (stateInfo.IsName("AttackR"))
            {
                anim.SetBool(Attack, false);
            }
        }
    }
    
    //Dead 모션
    public void AnimDead()
    {
        AnimCrossFade("Dead");
        bEnable = false;
    }
}
