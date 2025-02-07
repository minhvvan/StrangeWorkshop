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

    public override bool SetHoldableObjectParent(IHoldableObjectParent parent)
    {
        if (!parent.CanSetHoldableObject())
        {
            return false;
        }
        
        if (parent.GetType() == typeof(SampleCharacterController))
        {
            turretActions.Hold();
        }
        else
        {
            turretActions.Put();
        }
        
        return base.SetHoldableObjectParent(parent);
    }
    
    public override bool Acceptable(HoldableObject objectType)
    {
        if (turretData.isCrashed)
        {
            // 고장난 상태면 어떠한 것도 받지 못하도록 설정
            return false;
        }
        
        if (objectType.GetHoldableObjectSO().objectType == HoldableObjectType.Bullet)
        {
            // 남아있는 총알 개수에 관계없이 reload
            
            // 1. crash만 아니면 무조건 장전이 가능하도록 설정
            turretActions.Reload();
            return true;
            
            // 2. upgrade중이면 장전이 불가능하도록 설정
            // if (turretData.isUpgrading)
            // {
            //     return false;
            // }
        }
        else if (objectType.GetHoldableObjectSO().objectType == HoldableObjectType.Upgrade)
        {
            // upgrade 모듈이 놓였을때
            return turretActions.Upgrade();
        }
        
        // bullet이나 upgrade모듈이 아니면 return false
        return false;
    }
}