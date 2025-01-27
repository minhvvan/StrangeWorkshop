using System.Collections;
using System.Collections.Generic;
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
    }

    public override void UpdateState()
    {
        //target이 있으면 실행.
        if (Fsm.blackboard.target != null)
        {
            BlackboardEnemy FsmBb = Fsm.blackboard;
            Vector3 targetPos = FsmBb.target.position;
            Vector3 currentPos = FsmBb.transform.position;
            Vector3 direction = targetPos - currentPos;
            float moveSpd = FsmBb.enemyStatus.moveSpeed;
            //float distance = Vector3.Distance(targetPos, currentPos);
            
            //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
            Physics.Raycast(FsmBb.transform.position,FsmBb.transform.forward, out FsmBb.hit, 
                FsmBb.enemyStatus.attackRange,1 << LayerMask.NameToLayer(FsmBb.layerName));

            //target이 사거리보다 멀면 이동한다.
            if(FsmBb.hit.collider == null)
            {
                //타겟을 향해 이동한다.
                FsmBb.transform.rotation = Quaternion.LookRotation(direction);
                Vector3 movePos = Vector3.MoveTowards(currentPos, targetPos, moveSpd * Time.deltaTime);
                FsmBb.transform.position = movePos;
            }
            else
            {
                FsmBb.bDetectBarrier = true;
                //사거리 내에 들어 Attack으로 넘어간다
                Fsm.ChangeEnemyState(Fsm.attackState);
                return;
            }
        }
        else
        {
            //target없으면 Idle로 넘어간다.
            Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }
}
