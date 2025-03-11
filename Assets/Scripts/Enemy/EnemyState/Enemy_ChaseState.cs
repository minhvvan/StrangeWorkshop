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
        Fsm.blackboard.priorityIncrease = true;
        Fsm.blackboard.currentState = 1;
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

            float distance = Mathf.Infinity;
            if (Physics.Raycast(currentPos, direction.normalized, out FsmBb.hit, FsmBb.enemyStatus.attackRange,
                    1 << LayerMask.NameToLayer(FsmBb.layerName)))
            {
                distance = Vector3.Distance(FsmBb.hit.point, currentPos);
            }
            
            //Debug.Log($"{distance:F2},\n{targetPos},\n{currentPos}");
            //사거리 안에 들었을 때, 대상 탐지가 되지 않았다면 검색.
            if (distance <= FsmBb.enemyStatus.attackRange &&
                FsmBb.targetCollider == null)
            {
                //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
                FsmBb.SearchNearTarget();
            }
            
            //target이 사거리보다 멀면 이동한다.
            if(FsmBb.targetCollider == null)
            {
                //타겟을 향해 이동한다.
                FsmBb.transform.rotation = Quaternion.LookRotation(trForward);
                FsmBb.ResumeTracking();
                if (FsmBb.useAutoResearch)
                {
                    if (!FsmBb.researchOrder)
                    {
                        FsmBb.AutoResearchTarget().Forget();
                    }
                }
            }
            else
            {
                FsmBb.bDetectBarrier = true;
                FsmBb.researchOrder = false;
                FsmBb.autoResearchCts?.Cancel();
                FsmBb.StopTracking();
                
                //사거리 내에 들어 Attack으로 넘어간다
                Fsm.ChangeEnemyState(Fsm.attackState);
                return;
            }
        }
        else
        {
            //target없으면 Idle로 넘어간다.
            //FsmBb.ResearchTarget();
            Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }
}
