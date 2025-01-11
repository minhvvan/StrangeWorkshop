using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_AttackState : BaseState<Turret>
{
    public Turret_AttackState(Turret controller) : base(controller){ }

    public override void Enter()
    {
        Debug.Log("Enter AttackState");
    }

    public override void UpdateState()
    {
        ChangeState();
    }

    public override void Exit()
    {
        Debug.Log("Exit AttackState");
    }
    
    private void ChangeState()
    {
        /*
         State 변경 조건 확인
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         */
        if (!_controller.turret.isOnCounter)
        {
            Debug.Log("Change to HoldState");
            _controller.SetState(_controller.holdState);
        }
        else if (_controller.turret.isCrashed)
        {
            Debug.Log("Change to CrashState");
            _controller.SetState(_controller.crashState);
        }
        else if (_controller.turret.remainingBulletsNum <= 0)
        {
            Debug.Log("Change to EmptyState");
            _controller.SetState(_controller.emptyState);
        }
        else if (_controller.turret.target is null)
        {
            Debug.Log("Change to IdleState");
            _controller.SetState(_controller.idleState);
        }
    }
}