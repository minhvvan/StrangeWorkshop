using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class SampleCharacterController : MonoBehaviour, IHoldableObjectParent, IInteractAgent
{
    StateMachine _stateMachine;

    // ▼ State 캐싱
    [NonSerialized] public Character_IdleState idleState;
    [NonSerialized] public Character_WalkState walkState;
    [NonSerialized] public Character_RunState runState;
    [NonSerialized] public Character_DashState dashState;
    [NonSerialized] public Character_InteractState interactState;

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

    //캐릭터의 조작 제어
    [NonSerialized] public bool isMoveable = true; //캐릭터 이동가능 여부(스턴, 상태이상 등)
    [NonSerialized] public bool isInteracting = false; //캐릭터 상호작용 중 여부

    
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
    private IInteractable _selectedInteractable;
    
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
        interactState = new Character_InteractState(this);

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
        if (Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit interactObject, playerInteractDistance, playerInteractLayerMask))
        {
            if (interactObject.transform.TryGetComponent(out IInteractable interactable))
            {
                if (_selectedInteractable == interactable) return;
                
                _selectedInteractable?.GetGameObject().GetComponent<SelectObjectVisual>().Hide();
                _selectedInteractable = interactable;
                _selectedInteractable.GetGameObject().GetComponent<SelectObjectVisual>().Show();
            }
            else
            {
                ResetSelectedInteractable();
            }
        }
        else
        {
            ResetSelectedInteractable();
        }
    }

    private void ResetSelectedInteractable()
    {
        _selectedInteractable?.GetGameObject().GetComponent<SelectObjectVisual>().Hide();
        _selectedInteractable = null;
    }

    public IInteractable GetSelectedInteractableObject()
    {
        return _selectedInteractable;
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
        _holdableObject.SetHoldableObjectParentWithAnimation(parent);
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

    public GameObject GetGameObject()
    {
        return gameObject;
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


    /// <summary>
    /// 이동불가능한 상호작용의 시작을 위한 함수
    /// </summary>
    public void EnterInteraction()
    {
        isInteracting = true;
    }


    /// <summary>
    /// 이동불가능한 상호작용의 종료를 위한 함수
    /// </summary>
    public void ExitInteraction()
    {
        isInteracting = false;
    }


    /// <summary>
    /// 이동불가능한 상태의 시작을 위한 함수 (CC기 맞았을때 사용)
    /// </summary>
    public void EnterMoveable()
    {
        isMoveable = true;
        isInteracting = false;
    }

    /// <summary>
    /// 이동불가능한 상태의 종료를 위한 함수
    /// </summary>
    public void ExitMoveable()
    {
        isMoveable = false;

    }
}