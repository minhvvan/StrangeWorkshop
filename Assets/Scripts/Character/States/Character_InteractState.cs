using UnityEngine;

public class Character_InteractState : BaseState<SampleCharacterController>
{
    public Character_InteractState(SampleCharacterController controller) : base(controller) { }

    public override void Enter()
    {
        _controller.EnterInteraction();
        
    }

    public override void Exit()
    {
        _controller.ExitInteraction();
    }

    public override void UpdateState()
    {
        if(!_controller.isInteracting)
        {
            _controller.SetState(_controller.idleState);
        }
    }
}