using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
    public EnemyStatus enemyStatus;
    public Transform target;
    public RaycastHit hit;
    
    //비동기를 도중에 간섭할 수 있는 클래스.
    public CancellationTokenSource cts;

    //킬 스위치 개념.
    public bool bEnable = true;
    
    //방벽감지 및 공격
    private IAttackPattern _atkPattern;
    
    public string layerName = "Water";
    public bool bDetectBarrier = false;
    public bool bCanAttack = false;
    
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    //Material 오브젝트 할당용
    public Transform matObject;
    
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

        //Hierarchy에서 첫번째 자식(matertial이 적용된 자식)에게 접근
        foreach (Transform child in transform)
        {
            matObject = child;
            break;
        }
    }
    
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
    
    public void SetMaxHp()
    {
        enemyStatus.maxHp = enemyStatus.hp;
    }

    //적 공격종류 선택
    public void SetPattern()
    {
        _atkPattern = PatternHandler.CreatePattern(enemyStatus.enemytype);
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
        
        //공격 판정 동작.
        _atkPattern?.RunPattern(hit.collider, enemyStatus.attackDamage);
        
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
}
