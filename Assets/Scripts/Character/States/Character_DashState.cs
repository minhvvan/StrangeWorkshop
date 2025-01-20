using UnityEngine;

public class Character_DashState : BaseState<SampleCharacterController>
{
    public Character_DashState(SampleCharacterController controller) : base(controller) { }
    private float dashTimer;
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void UpdateState()
    {
        if(!_controller.isDashing)
        {
            _controller.SetState(_controller.runState);
        }
    }
}