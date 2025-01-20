using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 상호작용을 위한 클래스
/// OnInteract에 이벤트를 등록하여 사용
/// </summary>
[RequireComponent(typeof(SampleCharacterController))]
public class CharacterDash : BaseAction
{
    SampleCharacterController _controller;
    
    void Awake()
    {
        _controller = GetComponent<SampleCharacterController>();

        _controller.AddAction(this);
    }    
    
    public override bool RegistAction()
    {
        if(_controller.inputHandler == null) return false;

        _controller.inputHandler.OnDash += Dash; //단일 이벤트에 연결

        return true;
    }

    public override void UnregistAction()
    {
        if(_controller.inputHandler == null) return;

        _controller.inputHandler.OnDash -= Dash;        
    }

    void Dash()
    {
        if (!_controller.isDashing)
        { 
            _controller.SetState(_controller.dashState);
            StartCoroutine(DashRoutine());
        }
    }
    
    private IEnumerator DashRoutine()
    {
        CharacterInputHandler inputHandler = _controller.inputHandler;
        float dashSpeed = _controller.dashSpeed;
        float horizontal = inputHandler.Horizontal;
        float vertical = inputHandler.Vertical;
        
        Debug.Log("Dashing");
        _controller.isDashing = true;
        Vector3 movement = new Vector3(horizontal, 0, vertical);
        
        float accelTimer = 0f;
        while (accelTimer < _controller.dashAccelTime)
        {
            accelTimer += Time.deltaTime;
            float t = accelTimer / _controller.dashAccelTime;
            dashSpeed = Mathf.Lerp(_controller.walkSpeed, _controller.dashSpeed, t);
            movement = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y) * dashSpeed;
            transform.position += movement * Time.deltaTime;
            transform.forward = movement.normalized;
            yield return null;
        }
        float decelTimer = 0f;
        while (decelTimer < _controller.dashDecelTime)
        {
            decelTimer += Time.deltaTime;
            float t = decelTimer / _controller.dashDecelTime;
            dashSpeed = Mathf.Lerp(_controller.dashSpeed, _controller.walkSpeed, t);
            movement = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y) * dashSpeed;
            transform.position += movement * Time.deltaTime;
            transform.forward = movement.normalized;
            yield return null;
        }
        _controller.isDashing = false;
    }
}