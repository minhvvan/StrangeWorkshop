using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Turret : HoldableObject
{
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turretData { get; private set; }
    public Upgrade turretUpgrade { get; private set; }
    
    StateMachine _stateMachine;
    
    private bool _isInitialized = false;
    // turret 캐싱
    [NonSerialized] public Turret_IdleState idleState;
    [NonSerialized] public Turret_AttackState attackState;
    [NonSerialized] public Turret_NotWorkingState notWorkingState;
    // [NonSerialized] public Turret_CrashState crashState;
    
    async void Awake()
    {
        await UniTask.WaitUntil(() => QuestManager.Instance.IsInitialized && VFXManager.Instance.IsInitialized);
        
        InitComponents();
        InitStates();
        turretData.Initialize();
        
        // turretmanager에 해당 turret 추가
        TurretManager.Instance.AddTurret(this);
        _isInitialized = true;
    }

    private void InitComponents()
    {
        turretData = GetComponent<Blackboard_Turret>();
        turretUpgrade = GetComponent<Upgrade>();
    }
    
    private void InitStates()
    {
        _stateMachine = new StateMachine();
        
        idleState = new Turret_IdleState(this);
        attackState = new Turret_AttackState(this);
        notWorkingState = new Turret_NotWorkingState(this);
        // crashState = new Turret_CrashState(this);
        
        _stateMachine.ChangeState(idleState);
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    void Update()
    {
        if (!_isInitialized) return;
        
        TurretActions.UpdateTarget(this);
        _stateMachine.Update();
    }

    public override bool SetHoldableObjectParent(IHoldableObjectParent parent)
    {
        // if (!parent.CanSetHoldableObject())
        // {
        //     return false;
        // }
        //
        if (parent.GetType() == typeof(SampleCharacterController))
        {
            TurretActions.Hold(this);
        }
        else
        {
            TurretActions.Put(this, parent);
        }
        
        return base.SetHoldableObjectParent(parent);
    }
    
    public override bool Acceptable(HoldableObject objectType)
    {
        // if (turretData.isCrashed)
        // {
        //     // 고장난 상태면 어떠한 것도 받지 못하도록 설정
        //     return false;
        // }
        
        if (objectType.GetHoldableObjectSO().objectType == HoldableObjectType.Upgrade)
        {
            // upgrade 모듈이 놓였을때
            return TurretActions.Upgrade(this);
        }
        
        // if (objectType.GetHoldableObjectSO().objectType == HoldableObjectType.Bullet)
        // {
        //     // 남아있는 총알 개수에 관계없이 reload
        //     
        //     // 1. crash만 아니면 무조건 장전이 가능하도록 설정
        //     turretActions.Reload();
        //     return true;
        //     
        //     // 2. upgrade중이면 장전이 불가능하도록 설정
        //     // if (turretData.isUpgrading)
        //     // {
        //     //     return false;
        //     // }
        // }
        
        // bullet이나 upgrade모듈이 아니면 return false
        return false;
    }
}