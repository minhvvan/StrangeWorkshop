using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_IdleState : BaseState<Turret>
{
    public Turret_IdleState(Turret controller) : base(controller){ }

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
        /*
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         */
        if (!_controller.turretData.isOnCounter)
        {
            _controller.SetState(_controller.holdState);
        }
        else if (_controller.turretData.isCrashed)
        {
            _controller.SetState(_controller.crashState);
        }
        else if (_controller.turretData.currentBulletNum <= 0)
        {
            _controller.SetState(_controller.emptyState);
        }
        else if (_controller.turretData.target is not null)
        {
            _controller.SetState(_controller.attackState);
        }
    }
}
