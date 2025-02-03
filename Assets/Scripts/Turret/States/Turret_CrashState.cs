using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_CrashState : BaseState<Turret>
{
    private Blackboard_Turret _turretData;
    private Color[] _previousColors;
    public Turret_CrashState(Turret controller) : base(controller)
    {
        _turretData = _controller.turretData;
    }

    public override void Enter()
    {
        _previousColors = new Color[_turretData.renderers.Length];
        
        // 터렛 색 변경
        for (int i = 0; i < _turretData.renderers.Length; i++)
        {
            _previousColors[i] = _turretData.renderers[i].material.color;
            _turretData.renderers[i].material.color = _turretData.crashedColor;
        }
    }

    public override void UpdateState()
    {
        ChangeState();
    }

    public override void Exit()
    {
        // 원래 색으로
        for (int i = 0; i < _turretData.renderers.Length; i++)
        {
            _turretData.renderers[i].material.color = _previousColors[i];
        }
        
        // 메모리 해제
        _previousColors = null;
    }
    
    public void ChangeState()
    {        
        // 고장났는지 체크 -> 작동 가능한지 체크 -> target이 있는지 체크
        if (!_turretData.isCrashed)
        {
            if (!_turretData.isOnCounter || _turretData.currentBulletNum <= 0 || _turretData.isUpgrading)
            {
                _controller.SetState(_controller.notWorkingState);
            }
            else if (_turretData.target != null)
            {
                _controller.SetState(_controller.attackState);
            }
            else
            {
                _controller.SetState(_controller.idleState);
            }
        }
    }
}