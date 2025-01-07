using UnityEngine;

public class Character_WalkState : BaseState<CharacterController>
{
    public Character_WalkState(CharacterController controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("Walk Enter");
    }

    public override void Exit()
    {
        Debug.Log("Walk Exit");
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
    }
}