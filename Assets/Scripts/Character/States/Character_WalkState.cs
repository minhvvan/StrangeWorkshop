using UnityEngine;

public class Character_WalkState : BaseState<SampleCharacterController>
{
    public Character_WalkState(SampleCharacterController controller) : base(controller) { }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void UpdateState()
    {
        if (!_controller.inputHandler.IsWalking)
        {
            _controller.SetState(_controller.idleState);
        }
        else if (_controller.inputHandler.IsRunning)
        {
            _controller.SetState(_controller.runState);
        }
        else if (_controller.inputHandler.IsDashing)
        {
            _controller.SetState(_controller.dashState);
        }
    }
}