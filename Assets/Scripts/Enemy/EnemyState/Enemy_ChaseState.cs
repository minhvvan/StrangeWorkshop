using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy_ChaseState : BaseStateEnemy<EnemyFsm>
{
    public Enemy_ChaseState(EnemyFsm fsm) : base(fsm)
    {
        
    }
    
    public override void Enter()
    {
        Fsm.blackboard.AnimWalk();
        //Fsm.blackboard.NavControl(true).Forget();
    }

    public override void UpdateState()
    {
        BlackboardEnemy FsmBb = Fsm.blackboard;
        //target이 있으면 실행.
        if (Fsm.blackboard.target != null)
        {
            Vector3 targetPos = FsmBb.target.position;
            Vector3 currentPos = FsmBb.transform.position;
            Vector3 direction = targetPos - currentPos;
            Vector3 trForward = FsmBb.transform.forward = FsmBb.agent.transform.forward;
            
            float moveSpd = FsmBb.enemyStatus.moveSpeed;
  
            FsmBb.SearchNearTarget();

            //target이 사거리보다 멀면 이동한다.
            if(FsmBb.targetCollider == null)
            {
                //타겟을 향해 이동한다.
                FsmBb.transform.rotation = Quaternion.LookRotation(trForward);
                FsmBb.ResumeTracking();
            }
            else
            {
                FsmBb.bDetectBarrier = true;
                FsmBb.StopTracking();
                
                //사거리 내에 들어 Attack으로 넘어간다
                Fsm.ChangeEnemyState(Fsm.attackState);
                return;
            }
        }
        else
        {
            //target없으면 Idle로 넘어간다.
            FsmBb.ResearchTarget();
            //Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }
}
