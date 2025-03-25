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
        _controller.SetInputHandler(this);

        // 초기화 타이밍 문제 방지 (약간의 대기)
        yield return new WaitForSeconds(0.1f);
        _controller.SetInputHandler(this);
    }

    public override void ProcessInput(InputData input)
    {
        base.ProcessInput(input);
        
        //상태이상 활성화 시 종료
        if (!_controller.isMoveable) return;
        
        // 1) 이동 입력
        float horizontal = input.moveInput.X;
        float vertical   = input.moveInput.Y;

        MovementInput = new Vector2(horizontal, vertical).normalized;
        Horizontal    = horizontal;
        Vertical      = vertical;
        
        IsWalking = MovementInput.magnitude > 0.1f;

        if (input.interactPressed)
        {
            OnInteract?.Invoke();
        }

        if (input.interactAlternatePressed)
        {
            OnInteractAlternate?.Invoke();
        }

        if (input.dashPressed)
        {
            OnDash?.Invoke();
        }
    }

    public override void OnControlEnd()
    {
        base.OnControlEnd();
        
        _controller.rb.velocity = Vector3.zero;
        _controller.rb.angularVelocity = Vector3.zero;
        _controller.anim.SetFloat("Blend", 0f);
    }
}