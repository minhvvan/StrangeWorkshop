using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_IdleState : BaseStateEnemy<EnemyFsm>
{
    

    public Enemy_IdleState(EnemyFsm fsm) : base(fsm)
    {
        
    }
    
    public override void Enter()
    {
        Fsm.blackboard.AnimIdle();
        Fsm.blackboard.currentState = 0;
    }

    public override void UpdateState()
    {
        //target이 있으면 실행.
        if (Fsm.blackboard.target != null)
        {
            //Chase로 넘어간다.
            Fsm.ChangeEnemyState(Fsm.chaseState);
            return;
        }
    }

    public override void Exit()
    {
        
    }
}
