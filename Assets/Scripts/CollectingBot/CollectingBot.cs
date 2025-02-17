using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CollectingBot : MonoBehaviour
{
    public Transform ore;
    
    public Transform target;
    StateMachine _stateMachine;

    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public CollectingBot_CollectingState _collectingState;
    [NonSerialized] public CollectingBot_ChaseState _chaseState;
    [NonSerialized] public CollectingBot_IdleState _idleState;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitState();
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }

    void InitState()
    {
        _stateMachine = new StateMachine();

        _collectingState = new CollectingBot_CollectingState(this);
        _chaseState = new CollectingBot_ChaseState(this);
        _idleState = new CollectingBot_IdleState(this);
        
        SetState(_idleState);
    }

    public void SetState(IState state)
    {
        _stateMachine.ChangeState(state);
    }
}
