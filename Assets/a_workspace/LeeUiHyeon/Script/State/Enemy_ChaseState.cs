using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ChaseState : BaseStateEnemy<EnemyFsm>
{
    public Enemy_ChaseState(EnemyFsm fsm) : base(fsm)
    {
        
    }
    
    public override void Enter()
    {
        Debug.Log("Chase");
    }

    public override void UpdateState()
    {
        if (Fsm.blackboard.target != null)
        {
            
            Vector3 targetPos = Fsm.blackboard.target.position;
            Vector3 currentPos = Fsm.blackboard.transform.position;
            float moveSpd = Fsm.blackboard.enemyStatus.moveSpeed;
            
            float distance = Vector3.Distance(targetPos, currentPos);
            Vector3 direction = targetPos - currentPos;

            if (Fsm.blackboard.enemyStatus.attackRange < distance)
            {
                Fsm.blackboard.transform.rotation = Quaternion.LookRotation(direction);
                Vector3 movePos = Vector3.MoveTowards(currentPos, targetPos, moveSpd * Time.deltaTime);
                
                Fsm.blackboard.transform.position = movePos;
            }
            else
            {
                Fsm.ChangeEnemyState(Fsm.attackState);
                return;
            }
        }
        else
        {
            Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }

    public void MoveToTarget()
    {
        
    }
}
