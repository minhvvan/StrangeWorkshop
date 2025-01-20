using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy의 State를 제어하는 클래스입니다.
public class EnemyFsm : MonoBehaviour
{
    //enemy의 blackboard 접근
    private Enemy enemy;
    public BlackboardEnemy blackboard;
    
    //State캐싱
    private StateMachine _stateMachine;
    [NonSerialized] public Enemy_IdleState idleState;
    [NonSerialized] public Enemy_ChaseState chaseState;
    [NonSerialized] public Enemy_AttackState attackState;
    
    //List<BaseAction> enemyActions = new();
    
    void Awake()
    {
        InitComponents();
    }

    void InitComponents()
    {
        enemy = GetComponent<Enemy>();
    }
    
    public void InitStates()
    {
        _stateMachine = new StateMachine();

        //State초기화
        idleState = new (this);
        chaseState = new (this);
        attackState = new (this);

        //초기 State 설정
        _stateMachine.ChangeState(idleState);
        
        //블랙보드 할당
        blackboard = enemy.blackboard;
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
