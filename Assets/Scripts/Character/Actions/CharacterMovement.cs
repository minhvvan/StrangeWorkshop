using System;
using System.Collections;
using UnityEngine;

public enum CONTROLL_MODE
{
    FIRST_PERSON, //전진 + 회전 제어
    FIXED_CAMERA  //고정 카메라 제어
}

/// <summary>
/// 해당 클래스는 플레이어의 입력 세팅값에 따라 이동을 제어하는 클래스
/// Update 에서 지속적인 값을 체크하여 사용
/// </summary>

[RequireComponent(typeof(SampleCharacterController))]
public class CharacterMovement : BaseAction
{
    [SerializeField] public CONTROLL_MODE controlMode = CONTROLL_MODE.FIXED_CAMERA;

    SampleCharacterController _controller;
    
    void Awake()
    {
        _controller = GetComponent<SampleCharacterController>();

        _controller.AddAction(this);
    }

     public override bool RegistAction()
    {
        if(_controller.inputHandler == null) return false;

        _controller.inputHandler.OnActions += MoveCharacter; // 주기적으로 불려하는 내용은 Actions에 등록

        return true;
    }

    public override void UnregistAction()
    {
        if(_controller.inputHandler == null) return;

        _controller.inputHandler.OnActions -= MoveCharacter;
    }

    void MoveCharacter()
    {
        if(_controller.inputHandler == null) return;
        if(_controller.isDashing) return;
        
        CharacterInputHandler inputHandler = _controller.inputHandler;

        // 이동 속도 결정
        float speed = inputHandler.IsRunning ? _controller.runSpeed : _controller.walkSpeed;
        float horizontal = inputHandler.Horizontal;
        float vertical = inputHandler.Vertical;
        Vector3 movePos = Vector3.zero;
        
        if(controlMode == CONTROLL_MODE.FIXED_CAMERA)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            
            //if move position and direction 
            movement = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y).normalized * speed;
            
            
            transform.position += movement * Time.deltaTime;

            //look at
            if (inputHandler.MovementInput.magnitude > 0.1f)
            {
                transform.forward = movement.normalized;
            }
        }
        /*
        else if(controlMode == CONTROLL_MODE.FIRST_PERSON)
        {
            //forward and rotate
            if(vertical > 0)
            {
                movePos += transform.forward;
            }
            else if(vertical < 0)
            {
                movePos -= transform.forward;
            }

            if(horizontal > 0)
            {
                transform.Rotate(Vector3.up, 90 * Time.deltaTime);
            }
            else if(horizontal < 0)
            {
                transform.Rotate(Vector3.up, -90 * Time.deltaTime);
            }

            transform.position += movePos.normalized * speed * Time.deltaTime;
        }
        */
    }
}