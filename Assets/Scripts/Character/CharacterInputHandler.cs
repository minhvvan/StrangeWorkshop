using System;
using UnityEngine;

public class CharacterInputHandler : BaseInputHandler
{
    CharacterController _controller;

    //Events   //필요한 이벤트를 Action으로  추가하여 연결해서 class 붙여주면 코드 분리가 편하게 될 것으로 예상됨
    [NonSerialized] public Action OnInteract;  //만약 매개변수를 넘길경우 Action<자료형> 으로 추가하면 가능
    [NonSerialized] public Action OnAttack;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        OnActions += DirectControl;

        _controller.SetInputHandler(this);
    }
    
    protected override void Update(){
        //주기적으로 체크해야하는 항목에 대하여 base.Update위에 넣어서 사용  //예를 들면 CheckGrounded(이건 baseInputhandler에 넣어둠), CheckInteratable 등

        base.Update();
    }

    
    void DirectControl(){
        // 이동 방향 입력 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MovementInput = new Vector2(horizontal, vertical).normalized;
        Horizontal = horizontal;
        Vertical = vertical;

        // 키 입력 상태 갱신
        IsWalking = MovementInput.magnitude > 0.1f;
        IsJumping = IsGrounded && Input.GetKeyDown(KeyCode.Space);
        IsRunning = IsWalking && Input.GetKey(KeyCode.LeftShift);
        
        if(Input.GetMouseButtonDown(0)){
            OnAttack?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.E)){
            OnInteract?.Invoke();
        }
    }

    
}