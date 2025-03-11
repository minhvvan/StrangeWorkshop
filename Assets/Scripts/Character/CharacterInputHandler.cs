using System;
using System.Collections;
using UnityEngine;

public class CharacterInputHandler : BaseInputHandler
{
    SampleCharacterController _controller;

    // 필요 이벤트들
    [NonSerialized] public Action OnInteract;
    [NonSerialized] public Action OnInteractAlternate;
    [NonSerialized] public Action OnDash;

    IEnumerator Start()
    {
        _controller = GetComponent<SampleCharacterController>();
        OnActions += DirectControl;

        _controller.SetInputHandler(this);

        // 초기화 타이밍 문제 방지 (약간의 대기)
        yield return new WaitForSeconds(0.1f);
        _controller.SetInputHandler(this);
    }

    protected override void Update()
    {
        // 상위 BaseInputHandler 로직
        base.Update();
    }

    void DirectControl()
    {
        //상태이상 활성화 시 종료
        if (!_controller.isMoveable) return;
        
        // 1) 이동 입력
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        MovementInput = new Vector2(horizontal, vertical).normalized;
        Horizontal    = horizontal;
        Vertical      = vertical;
        
        IsWalking = MovementInput.magnitude > 0.1f;

        // 3) 상호작용
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteract?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnInteractAlternate?.Invoke();
        }

        // 4) 대쉬
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDash?.Invoke();
        }
    }
}