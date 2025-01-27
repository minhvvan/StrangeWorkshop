using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    public string layerName = "Water";
    public bool bDetectBarrier = false;
    public bool bCanAttack = false;
    
    private static readonly int _roSpeed = Animator.StringToHash("Speed");
    private static readonly int _roAttack = Animator.StringToHash("Attack");
    
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
    }
    
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
    
    //CrossFade 간소화.
    public void EnmCrossFade(string animName)
    {
        anim.CrossFade(animName, 0.1f);
    }
    
    //Idle 모션
    public void AnimIdle()
    {
        if (anim.name != "Idle")
        {
            EnmCrossFade("Idle");
        }
        anim.SetFloat(_roSpeed, 0.0f);
    }

    //Walk 모션
    public void AnimWalk()
    {
        if (anim.name != "Idle")
        {
            EnmCrossFade("Idle");
        }
        anim.SetFloat(_roSpeed, 1.0f);
    }

    //Attack 모션
    public void AnimAttack()
    {
        //걷기 중단.
        anim.SetFloat(_roSpeed, 0.0f);

        //좌우 번갈아가며 공격하는 모션
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("AttackL") && !stateInfo.IsName("AttackR"))
        {
            EnmCrossFade("AttackL");
            anim.SetBool(_roAttack, false);
        }
        else
        {
            if (stateInfo.IsName("AttackL"))
            {
                anim.SetBool(_roAttack, true);
            }
            else if (stateInfo.IsName("AttackR"))
            {
                anim.SetBool(_roAttack, false);
            }
        }
    }
    
    //Dead 모션
    public void AnimDead()
    {
        EnmCrossFade("Dead");
        bEnable = false;
    }
}
