using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFsm : MonoBehaviour
{
    private IBlackboardEnemy _blackboardEnemy;
    public BlackboardEnemy blackboard;
    StateMachine _stateMachine;
    
    //State캐싱
    [NonSerialized] public Enemy_IdleState idleState;
    [NonSerialized] public Enemy_ChaseState chaseState;
    [NonSerialized] public Enemy_AttackState attackState;
    
    List<BaseAction> enemyActions = new();
    
    public Rigidbody rb;
    public Animator anim;

    void Awake()
    {
        InitComponents();
        InitStates();
    }

    void InitComponents()
    {
        _blackboardEnemy = GetComponent<IBlackboardEnemy>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    
    public void InitStates()
    {
        _stateMachine = new StateMachine();

        //State초기화
        idleState = new (this);
        chaseState = new (this);
        attackState = new (this);

        
        _blackboardEnemy.InitBlackboard();
        blackboard = _blackboardEnemy as BlackboardEnemy;

        //초기 State 설정
        _stateMachine.ChangeState(idleState);
    }
    
    public void ChangeEnemyState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
    
    public void Update()
    {
        _stateMachine.Update();
    }
}
