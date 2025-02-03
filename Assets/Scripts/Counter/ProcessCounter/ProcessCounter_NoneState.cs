using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCounter_NoneState : BaseState<ProcessCounter>
{
    public ProcessCounter_NoneState(ProcessCounter controller) : base(controller) {}
    
    public override void Enter()
    {
        //UI 끄기
        _controller.progressBar.gameObject.SetActive(false);
    }

    public override void UpdateState()
    {
        if(_controller.isWork)
            _controller.SetState(_controller._processingState);
    }

    public override void Exit()
    {
        
    }
}