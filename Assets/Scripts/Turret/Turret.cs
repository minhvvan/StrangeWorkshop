using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : HoldableObject
{
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turretData { get; private set; }
    public Upgrade turretUpgrade { get; private set; }
    public TurretActions turretActions { get; private set; }
    
    StateMachine _stateMachine;
    
    // turret 캐싱
    [NonSerialized] public Turret_IdleState idleState;
    [NonSerialized] public Turret_AttackState attackState;
    [NonSerialized] public Turret_NotWorkingState notWorkingState;
    [NonSerialized] public Turret_CrashState crashState;
    
    void Awake()
    {
        InitComponents();
        InitStates();
        turretData.Initialize();
        
        // turretmanager에 해당 turret 추가
        TurretManager.Instance.AddTurret(this);
    }

    private void InitComponents()
    {
        turretData = GetComponent<Blackboard_Turret>();
        turretUpgrade = GetComponent<Upgrade>();
        turretActions = new TurretActions(this);
    }
    
    private void InitStates()
    {
        _stateMachine = new StateMachine();
        
        idleState = new Turret_IdleState(this);
        attackState = new Turret_AttackState(this);
        notWorkingState = new Turret_NotWorkingState(this);
        crashState = new Turret_CrashState(this);
        
        _stateMachine.ChangeState(notWorkingState);
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    void Update()
    {
        turretActions.UpdateTarget();
        _stateMachine.Update();
    }
}