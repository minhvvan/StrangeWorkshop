using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class Turret : HoldableObject
{
    // public UpgradeDataSO upgradeData;
    
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turretData { get; private set; }
    public Upgrade turretUpgrade { get; private set; }
    
    StateMachine _stateMachine;
    
    [NonSerialized] public bool IsInitialized = false;
    // turret 캐싱
    [NonSerialized] public Turret_IdleState idleState;
    [NonSerialized] public Turret_AttackState attackState;
    [NonSerialized] public Turret_NotWorkingState notWorkingState;
    
    async void Awake()
    {
        await UniTask.WaitUntil(() => QuestManager.Instance.IsInitialized && VFXManager.Instance.IsInitialized);
        
        InitComponents();
        InitStates();
        turretData.Initialize();
        
        // turretmanager에 해당 turret 추가
        TurretManager.Instance.AddTurret(this);
        IsInitialized = true;

        // TestUpgrade();
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
        
        _stateMachine.ChangeState(idleState);
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    void Update()
    {
        if (!IsInitialized) return;
        _stateMachine.Update();
    }

    void FixedUpdate()
    {
        if (!IsInitialized) return;
        TurretActions.UpdateTarget(this);
    }

    void OnDestroy()
    {
        TurretManager.Instance.RemoveTurret(this);
    }

    public override bool SetHoldableObjectParent(IHoldableObjectParent parent)
    {
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
    
    public override bool Acceptable(HoldableObject holdableObject)
    {
        UpgradeDataSO upgradeData = ((UpgradeModuleObject)holdableObject).GetUpgradeDataSO();
        if (holdableObject.GetHoldableObjectSO().objectType == HoldableObjectType.Upgrade && !turretData.isUpgrading)
        {
            
            // upgrade 모듈이 놓였을때
            TurretActions.Upgrade(this, upgradeData);
            return true;
        }
        
        // upgrade module이 아니면 return false
        return false;
    }

    // private async UniTask TestUpgrade()
    // {
    //     await UniTask.WaitForSeconds(15);
    //     TurretActions.Upgrade(this, upgradeData);
    // }
}