using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // public GameObject enemy;
    // 터렛 정보를 담고있는 블랙보드
    public Blackboard_Turret turret { get; private set; }
    
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
        // for debugging
        // StartCoroutine(StateChangeDebugging());
    }

    private void InitComponents()
    {
        turret = GetComponent<Blackboard_Turret>();
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

    void Update()
    {
        _stateMachine.Update();
    }
    
    // IEnumerator StateChangeDebugging()
    // {
    //     // 예상되는 state 변화 idle -> attack -> crash -> hold -> empty -> attack -> idle -> hold
    //     yield return new WaitForSeconds(2f);
    //     turret.target = enemy;
    //     yield return new WaitForSeconds(2f);
    //     turret.isCrashed = true;
    //     yield return new WaitForSeconds(2f);
    //     turret.remainingBulletsNum = 0;
    //     yield return new WaitForSeconds(2f);
    //     turret.isOnCounter = false;
    //     yield return new WaitForSeconds(2f);
    //     turret.isOnCounter = true;
    //     turret.isCrashed = false;
    //     yield return new WaitForSeconds(2f);
    //     turret.remainingBulletsNum = 50;
    //     yield return new WaitForSeconds(2f);
    //     turret.target = null;
    //     yield return new WaitForSeconds(2f);
    //     turret.isOnCounter = false;
    //     turret.isCrashed = true;
    //     turret.target = enemy;
    //     turret.remainingBulletsNum = 0;
    // }
}