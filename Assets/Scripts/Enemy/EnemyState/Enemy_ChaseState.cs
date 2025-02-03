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
            //float distance = Vector3.Distance(targetPos, currentPos);
            
            //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
            Physics.Raycast(FsmBb.transform.position,trForward, out FsmBb.hit, 
                FsmBb.enemyStatus.attackRange,1 << LayerMask.NameToLayer(FsmBb.layerName));
            // FsmBb.hitColliders = Physics.OverlapSphere(FsmBb.transform.position, FsmBb.enemyStatus.attackRange, 
            //     1 << LayerMask.NameToLayer(FsmBb.layerName));

            //target이 사거리보다 멀면 이동한다.
            if(FsmBb.hit.collider == null)
            //if(FsmBb.hitColliders == null)
            {
                //타겟을 향해 이동한다.
                FsmBb.transform.rotation = Quaternion.LookRotation(trForward);
                //Vector3 movePos = Vector3.MoveTowards(currentPos, targetPos, moveSpd * Time.deltaTime);
                //FsmBb.transform.position = movePos;
                FsmBb.ResumeTracking();
            }
            else //if(FsmBb.hit.transform == FsmBb.target)
            {
                FsmBb.bDetectBarrier = true;
                FsmBb.StopTracking();
                //FsmBb.transform.forward = FsmBb.hit.point.normalized;
                
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
