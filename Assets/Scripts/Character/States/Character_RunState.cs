using UnityEngine;

public class Character_RunState : BaseState<SampleCharacterController>
{
    public Character_RunState(SampleCharacterController controller) : base(controller) { }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void UpdateState()
    {
        if(!_controller.inputHandler.IsRunning)
        {
            _controller.SetState(_controller.walkState);
        }
    }
}