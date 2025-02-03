using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_IdleState : BaseState<Turret>
{
    private Blackboard_Turret _turretData;

    public Turret_IdleState(Turret controller) : base(controller)
    {
        _turretData = _controller.turretData;
    }

    public override void Enter()
    {
    }
    
    public override void UpdateState()
    {
        ChangeState();
    }

    public override void Exit()
    {
    }

    private void ChangeState()
    {
        // 고장났는지 체크 -> 작동 가능한지 체크 -> target이 있는지 체크
        if (_turretData.isCrashed)
        {
            _controller.SetState(_controller.crashState);
        }
        else if (!_turretData.isOnCounter || _turretData.currentBulletNum <= 0 || _turretData.isUpgrading)
        {
            _controller.SetState(_controller.notWorkingState);
        }
        else if (_turretData.target != null)
        {
            _controller.SetState(_controller.attackState);
        }
    }
}
