using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectingBot_ChaseState : BaseState<CollectingBot>
{
    public CollectingBot_ChaseState(CollectingBot controller) : base(controller){}

    private float time;
    
    public override void Enter()
    {
        _controller.agent.SetDestination(_controller.target.position);
    }

    public override void UpdateState()
    {
        time += Time.deltaTime;
        
        if (!_controller.agent.hasPath && time >= 0.5f)
        {
            if (_controller.target.gameObject.layer == LayerMask.NameToLayer("Barrier"))
            {
                _controller.target = _controller.ore;
                _controller.SetState(this);
            }
            else if (_controller.target.gameObject.layer == LayerMask.NameToLayer("Ore"))
            {
                _controller.SetState(_controller._collectingState);
            }
        }
    }

    public override void Exit()
    {
        time = 0;
    }
}