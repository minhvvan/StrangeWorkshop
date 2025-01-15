using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turretData { get; private set; }
    public TurretActions turretActions { get; private set; }
    
    StateMachine _stateMachine;
    
    // turret 캐싱
    [NonSerialized] public Turret_IdleState idleState;
    [NonSerialized] public Turret_AttackState attackState;
    [NonSerialized] public Turret_HoldState holdState;
    [NonSerialized] public Turret_EmptyState emptyState;
    [NonSerialized] public Turret_CrashState crashState;
    
    void Awake()
    {
        InitComponents();
        InitStates();
        InitTurretData();
    }

    private void InitComponents()
    {
        turretData = GetComponent<Blackboard_Turret>();
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

    private void InitTurretData()
    {
        turretActions = new TurretActions(this);
        turretActions.Reload();
        turretActions.Fix();
        turretActions.SetTargetStrategy(new ClosestTargetStrategy());
        turretActions.SetShootingStrategy(new SingleShootingStrategy(this));
        turretActions.SetRangeEffectSize();
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    void Update()
    {
        UpdateTarget();
        _stateMachine.Update();
        turretActions.ReduceHealth();
        if (turretData.currentHealth <= 0)
        {
            turretActions.Crash();
        }
    }

    void UpdateTarget()
    {
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, turretData.attackRange, layerMask);

        // no enemy in range
        if (hitColliders.Length <= 0)
        {
            turretData.target = null;
            return;
        }
        turretData.target = turretData.targetStrategy.SelectTarget(hitColliders, this);
    }
}