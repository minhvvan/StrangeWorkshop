using System;
using System.Collections.Generic;
using UnityEngine;


public class SampleCharacterController : MonoBehaviour, IHoldableObjectParent
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

    [SerializeField] private Transform holdableObjectHoldPoint;
    private Transform gloveObject;
    private HoldableObject _holdableObject;
    private BaseCounter _selectedCounter;

    [SerializeField] float playerInteractDistance = 1f;
    [SerializeField] LayerMask playerInteractLayerMask;
    
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
                    
                }
                else{
                    
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
        HandleInteract();
    }

    private void HandleInteract()
    {
        if (Physics.Raycast(transform.position, transform.forward + (Vector3.down * (transform.position.y / 2)), out RaycastHit interactObject, playerInteractDistance))
        {
            if (interactObject.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != _selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter counter)
    {
        if (counter == _selectedCounter) return;
        
        _selectedCounter?.GetComponent<SelectCounterVisual>().Hide();
        _selectedCounter = counter;
        _selectedCounter?.GetComponent<SelectCounterVisual>().Show();
    }

    public BaseCounter GetSelectedCounter()
    {
        return _selectedCounter;
    }
    
    public Transform GetHoldableObjectFollowTransform()
    {
        return holdableObjectHoldPoint;
    }

    public void SetHoldableObject(HoldableObject holdableObject)
    {
        this._holdableObject = holdableObject;
    }

    public void GiveHoldableObject(IHoldableObjectParent parent)
    {
        _holdableObject.SetHoldableObjectParent(parent);
        _holdableObject = null;
    }

    public HoldableObject GetHoldableObject()
    {
        return _holdableObject;
    }

    public void ClearHoldableObject()
    {
        Destroy(_holdableObject.gameObject);
        _holdableObject = null;
    }

    public bool HasHoldableObject()
    {
        return _holdableObject != null;
    }

    public bool CanSetHoldableObject()
    {
        return gloveObject != null;
    }

    public void WearGlove(Transform glove)
    {
        glove.parent = GetHoldableObjectFollowTransform();
        gloveObject = glove;
    }

    public void TakeoffGlove()
    {
        if (gloveObject != null)
            Destroy(gloveObject.gameObject);
        gloveObject = null;
    }
}