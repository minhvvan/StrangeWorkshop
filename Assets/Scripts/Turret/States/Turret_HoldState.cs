using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_HoldState : BaseState<Turret>
{
    public Turret_HoldState(Turret controller) : base(controller){ }

    public override void Enter()
    {
        Debug.Log("Enter HoldState");
    }

    public override void UpdateState()
    {
        ChangeState();
    }

    public override void Exit()
    {
        Debug.Log("Exit HoldState");
    }
    
    private void ChangeState()
    {
        /*
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         e.g. turret이 고장난 상태에서 플레이어가 turret을 들면 무조건 holdstate가 된다.
         */
        if (_controller.turret.isOnCounter)
        {
            if (_controller.turret.isCrashed)
            {
                Debug.Log("Change to CrashState");
                _controller.SetState(_controller.crashState);
            }
            else if (_controller.turret.remainingBulletsNum <= 0)
            {
                Debug.Log("Change to EmptyState");
                _controller.SetState(_controller.emptyState);
            }
            else if (_controller.turret.target is not null)
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