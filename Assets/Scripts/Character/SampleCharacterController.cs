using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class SampleCharacterController : MonoBehaviour, IHoldableObjectParent
{
    StateMachine _stateMachine;

    // ▼ State 캐싱
    [NonSerialized] public Character_IdleState idleState;
    [NonSerialized] public Character_WalkState walkState;
    [NonSerialized] public Character_RunState runState;
    [NonSerialized] public Character_DashState dashState;

    // ▼ 사용하는 액션들
    List<BaseAction> _actions = new List<BaseAction>();

    // ▼ 공통 변수
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public Animator anim;
    [SerializeField] private RuntimeAnimatorController defaultAnimatorController;
    [SerializeField] private RuntimeAnimatorController holdingOverrideController;
    
    [NonSerialized] public CharacterInputHandler inputHandler;

    [Header("Speed Settings")]
    public float walkSpeed = 15f;
    public float runSpeed  = 10f;
    public float dashSpeed = 30f; // 대쉬 속도

    [NonSerialized] public bool isMoveable = true;
    
    [Header("Dash Timings")]
    [SerializeField] public float dashAccelTime = 0.5f;
    [SerializeField] public float dashDecelTime = 0.5f;
    
    [NonSerialized] public bool isDashing = false;
    
    [Header("Movement Settings")]
    [Tooltip("회전 보간 속도")]
    public float desiredRotationSpeed = 0.1f;

    [Tooltip("입력 벡터가 이 값보다 클 때만 이동/회전 처리(애니메이션 전환 등)")]
    public float allowPlayerRotation = 0.1f;

    [Header("Animation Smoothing")]
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime  = 0.15f;

    // 예: 물건을 들 때 사용되는 위치
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
        // Rigidbody / Animator는 자식 개체에 있거나, 동일 오브젝트에 있을 수 있음
        rb   = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        if (defaultAnimatorController == null)
        {
            defaultAnimatorController = anim.runtimeAnimatorController;
        }
    }

    void InitStates()
    {
        _stateMachine = new StateMachine();

        // State 초기화
        idleState = new Character_IdleState(this);
        walkState = new Character_WalkState(this);
        runState  = new Character_RunState(this);
        dashState = new Character_DashState(this);

        // 초기 상태
        _stateMachine.ChangeState(idleState);
    }

    public void AddAction(BaseAction action)
    {
        _actions.Add(action);
    }

    public void SetInputHandler(BaseInputHandler inputHandler)
    {
        // 이미 연결돼 있던 핸들러가 있으면 먼저 해제
        if (this.inputHandler != null)
        {
            foreach (var action in _actions)
            {
                action.UnregistAction();
            }
        }

        this.inputHandler = inputHandler as CharacterInputHandler;

        // 새로 연결된 핸들러에 액션 등록
        if (this.inputHandler != null)
        {
            foreach (var action in _actions)
            {
                action.RegistAction();
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
        if (HasHoldableObject())
        {
            anim.runtimeAnimatorController = holdingOverrideController;
        }
        else
        {
            anim.runtimeAnimatorController = defaultAnimatorController;
        }
    }

    private void HandleInteract()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit interactObject, playerInteractDistance))
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