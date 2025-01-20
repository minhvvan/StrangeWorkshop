using System;
using System.Collections.Generic;
using UnityEngine;


public class SampleCharacterController : MonoBehaviour
{
    StateMachine _stateMachine;

    //State를 매번 새로 생성하지 않기위하여 캐싱
    [NonSerialized] public Character_IdleState idleState;
    [NonSerialized] public Character_WalkState walkState;
    [NonSerialized] public Character_RunState runState;
    [NonSerialized] public Character_DashState dashState;

    //사용할 액션들을 저장하고 등록하기 위함
    List<BaseAction> _actions = new List<BaseAction>();


    //공통적으로 사용할 변수 (Blackboard로 전체를 묶어서 사용할 수 있음)
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public Animator anim;
    
    [NonSerialized] public CharacterInputHandler inputHandler;

    [Header("Speed Settings")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float runSpeed = 10f;
    [SerializeField] public float dashSpeed = 15f; // 대쉬 시 최고 속도
    
    [Header("Dash Timings")]
    [SerializeField] public float dashAccelTime = 0.5f;
    [SerializeField] public float dashDecelTime = 0.5f;
    
    [NonSerialized] public bool isDashing = false;


    void Awake()
    {
        InitComponents();
        InitStates();
    }

    void InitComponents()
    {
        //하위에 하나만 있을 경우를 생각 범위에 관련된 부분은 (Collider) 생각해봅시다
        rb = GetComponentInChildren<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }


    void InitStates(){
        _stateMachine = new StateMachine();

        //State초기화
        idleState = new Character_IdleState(this);
        walkState = new Character_WalkState(this);
        runState = new Character_RunState(this);
        dashState = new Character_DashState(this);

        //초기 State 설정
        _stateMachine.ChangeState(new Character_IdleState(this));
    }

    public void AddAction(BaseAction action){
        _actions.Add(action);
    }

    public void SetInputHandler(BaseInputHandler inputHandler){
        if(this.inputHandler != null){
            foreach (var action in _actions)
            {
                action.UnregistAction();
            }
        }

        this.inputHandler = inputHandler as CharacterInputHandler;
        if(inputHandler != null){
            foreach (var action in _actions)
            {
                if(action.RegistAction()){
                    Debug.Log("Action Regist Success");
                }
                else{
                    Debug.Log("Action Regist Fail");
                }
            }
        }
    }

    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }


    void Update()
    {
        _stateMachine.Update();
    }
}