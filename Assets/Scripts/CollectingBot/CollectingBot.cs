using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CollectingBot : MonoBehaviour, IHoldableObjectParent, IInteractAgent
{
    /* TODO: 직접할당이 아닌 Ore찾는 Method불러오기(PathFinder 이용), collectCounter find로 가져오기(씬에 하나만 있다고 가정)
    */
    public Transform ore;
    public Transform target;
    public BaseCounter collectCounter;
    
    public Transform holdableObjectHoldPoint;
    
    StateMachine _stateMachine;

    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public CollectingBot_CollectingState _collectingState;
    [NonSerialized] public CollectingBot_ChaseState _chaseState;
    [NonSerialized] public CollectingBot_IdleState _idleState;
    [NonSerialized] public HoldableObject holdableObject;
    
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

    public Transform GetHoldableObjectFollowTransform()
    {
        return holdableObjectHoldPoint;
    }

    public void SetHoldableObject(HoldableObject holdableObject)
    {
        this.holdableObject = holdableObject;
    }

    public void GiveHoldableObject(IHoldableObjectParent parent)
    {
        holdableObject.SetHoldableObjectParentWithAnimation(parent);
        holdableObject = null;
    }

    public HoldableObject GetHoldableObject()
    {
        return holdableObject;
    }

    public void ClearHoldableObject()
    {
        Destroy(holdableObject.gameObject);
        holdableObject = null;
    }

    public bool HasHoldableObject()
    {
        return holdableObject != null;
    }

    public bool CanSetHoldableObject()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
