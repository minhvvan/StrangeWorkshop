using Unity.VisualScripting;
using UnityEngine;

public class CollectingBot_IdleState : BaseState<CollectingBot>
{
    public CollectingBot_IdleState(CollectingBot controller) : base(controller){}


    public override void Enter()
    {
    }

    public override void UpdateState()
    {
        if (!_controller.target.IsUnityNull())
        {
            _controller.SetState(_controller._chaseState);
        }
    }

    public override void Exit()
    {
    }
}