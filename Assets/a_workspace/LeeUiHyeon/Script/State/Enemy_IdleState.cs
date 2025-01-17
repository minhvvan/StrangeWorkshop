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
        Debug.Log("Idle");
    }

    public override void UpdateState()
    {
        if (Fsm.blackboard.target != null)
        {
            Fsm.ChangeEnemyState(Fsm.chaseState);
            return;
        }
    }

    public override void Exit()
    {
        
    }
}
