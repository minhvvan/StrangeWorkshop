using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackState : BaseStateEnemy<EnemyFsm>
{
    public Enemy_AttackState(EnemyFsm fsm) : base(fsm)
    {
        
    }
    
    public override void Enter()
    {
        Debug.Log("Attack");
    }

    public override void UpdateState()
    {
        if (Fsm.blackboard.target != null)
        {
            Vector3 targetPos = Fsm.blackboard.target.position;
            Vector3 currentPos = Fsm.blackboard.transform.position;
            
            Vector3 direction = targetPos - currentPos;
            float distance = Vector3.Distance(targetPos, currentPos);
            
            if (Fsm.blackboard.enemyStatus.attackRange < distance)
            {
                Fsm.ChangeEnemyState(Fsm.chaseState);
                return;
            }
            Debug.Log("Keep Attack");
        }
        else
        {
            Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }

    public async void OnAttack()
    {
        
    }
}
