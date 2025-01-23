using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCounter_NoneState : BaseState<ProcessCounter>
{
    public ProcessCounter_NoneState(ProcessCounter controller) : base(controller) {}
    
    public override void Enter()
    {
        //UI 끄기
    }

    public override void UpdateState()
    {
    }

    public override void Exit()
    {
    }
}