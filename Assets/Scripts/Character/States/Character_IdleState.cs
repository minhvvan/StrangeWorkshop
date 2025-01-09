using UnityEngine;

public class Character_IdleState : BaseState<SampleCharacterController>
{
    public Character_IdleState(SampleCharacterController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("Idle Enter");
    }

    public override void Exit()
    {
        Debug.Log("Idle Exit");        
    }

    public override void UpdateState()
    {
        if (_controller.inputHandler.IsWalking)
        {
            _controller.SetState(_controller.walkState);
        }
        else if (_controller.inputHandler.IsRunning)
        {
            _controller.SetState(_controller.runState);
        }
    }
}