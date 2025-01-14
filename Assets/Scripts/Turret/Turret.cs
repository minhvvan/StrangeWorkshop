using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turret { get; private set; }
    
    StateMachine _stateMachine;
    
    // turret 캐싱
    [NonSerialized] public Turret_IdleState idleState;
    [NonSerialized] public Turret_AttackState attackState;
    [NonSerialized] public Turret_HoldState holdState;
    [NonSerialized] public Turret_EmptyState emptyState;
    [NonSerialized] public Turret_CrashState crashState;

    // turret status
    [NonSerialized] public GameObject target;
    [NonSerialized] public int remainingBulletsNum;
    [NonSerialized] public bool isOnCounter = true;
    [NonSerialized] public bool isCrashed = false;
    
    void Awake()
    {
        InitComponents();
        InitStates();
        SetRangeEffectSize();
    }

    private void InitComponents()
    {
        turret = GetComponent<Blackboard_Turret>();
        remainingBulletsNum = turret.maxBulletNum;
        SetTargetStrategy(new ClosestTargetStrategy());
        turret.shootingStrategy = new SingleShootingStrategy(this);
    }
    private void InitStates()
    {
        _stateMachine = new StateMachine();
        
        idleState = new Turret_IdleState(this);
        attackState = new Turret_AttackState(this);
        holdState = new Turret_HoldState(this);
        emptyState = new Turret_EmptyState(this);
        crashState = new Turret_CrashState(this);
        
        _stateMachine.ChangeState(idleState);
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void SetTargetStrategy(ITargetStrategy newStrategy)
    {
        turret.targetStrategy = newStrategy;
    }

    public void SetShootingStrategy(ShootingStrategy newStrategy)
    {
        turret.shootingStrategy = newStrategy;
    }

    public void SetRangeEffectSize()
    {
        turret.rangeEff.transform.localScale = new Vector3(turret.attackRange * 2f, turret.attackRange * 2f, 1f);
    }
    
    void Update()
    {
        UpdateTarget();
        _stateMachine.Update();
    }

    void UpdateTarget()
    {
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, turret.attackRange, layerMask);

        // no enemy in range
        if (hitColliders.Length <= 0)
        {
            target = null;
            return;
        }
        target = turret.targetStrategy.SelectTarget(hitColliders, gameObject);
    }
}