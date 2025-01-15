using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_CrashState : BaseState<Turret>
{
    private Color[] _previousColors;
    public Turret_CrashState(Turret controller) : base(controller){ }

    public override void Enter()
    {
        Debug.Log("Enter CrashState");
        _previousColors = new Color[_controller.turretData.renderers.Length];
        
        // 터렛 색 변경
        for (int i = 0; i < _controller.turretData.renderers.Length; i++)
        {
            _previousColors[i] = _controller.turretData.renderers[i].material.color;
            _controller.turretData.renderers[i].material.color = _controller.turretData.crashedColor;
        }
        _controller.turretData.rangeEff.SetActive(false);
    }

    public override void UpdateState()
    {
        ChangeState();
    }

    public override void Exit()
    {
        Debug.Log("Exit CrashState");
        // 원래 색으로
        for (int i = 0; i < _controller.turretData.renderers.Length; i++)
        {
            _controller.turretData.renderers[i].material.color = _previousColors[i];
        }
        _controller.turretData.rangeEff.SetActive(true);
        
        // 메모리 해제
        _previousColors = null;
    }
    
    private void ChangeState()
    {
        /*
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         */
        if (!_controller.turretData.isOnCounter)
        {
            Debug.Log("Change to HoldState");
            _controller.SetState(_controller.holdState);
        }
        else if (!_controller.turretData.isCrashed)
        {
            if (_controller.turretData.curretBulletNum <= 0)
            {
                Debug.Log("Change to EmptyState");
                _controller.SetState(_controller.emptyState);
            }
            else if (_controller.turretData.target is not null)
            {
                Debug.Log("Change to AttackState");
                _controller.SetState(_controller.attackState);
            }
            else
            {
                Debug.Log("Change to IdleState");
                _controller.SetState(_controller.idleState);
            }
        }
    }
}