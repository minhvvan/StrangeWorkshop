using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy_AttackState : BaseStateEnemy<EnemyFsm>
{
    public Enemy_AttackState(EnemyFsm fsm) : base(fsm)
    {
        
    }
    
    public override void Enter()
    {
        //공격도중(정지시) 적들간 밀림방지. 이동중인 개체는 상관없음.
        EnemyPathfinder.instance.ColliderDisable(Fsm.blackboard.capsuleCol);
        Fsm.blackboard.priorityIncrease = false;
        Fsm.blackboard.agent.avoidancePriority = 1;
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
            Physics.Raycast(currentPos, direction, out FsmBb.hit, FsmBb.enemyStatus.attackRange,
                1 << LayerMask.NameToLayer(FsmBb.layerName));
            float distance = Vector3.Distance(FsmBb.hit.point, currentPos);
            //사거리 안에 들었을 때, 대상 탐지가 되지 않았다면 검색.
            if (distance <= FsmBb.enemyStatus.attackRange &&
                FsmBb.targetCollider == null)
            {
                //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
                FsmBb.SearchNearTarget();
            }
            //거리가 멀어지면 다시 Chase로 가도록 타겟을 비운다
            else if(distance > FsmBb.enemyStatus.attackRange &&
                    FsmBb.targetCollider != null)
            {
                FsmBb.targetCollider = null;
            }
            //target과의 거리가 사거리보다 길면 Chase로 이동.
            if(FsmBb.targetCollider == null)
            {
                FsmBb.bCanPattern = false;
                FsmBb.cts?.Cancel();
                FsmBb.cts = new CancellationTokenSource();
                FsmBb.AnimSetSpeed(1f);
                
                //공격이 끝나지 않았다면 잠시 대기.
                if (!FsmBb.bCanPattern)
                {
                    FsmBb.bDetectBarrier = false;
                    Fsm.ChangeEnemyState(Fsm.chaseState);
                    return;
                }
            }
            
            //적 공격 상태를 유지한다.
            if (!FsmBb.bCanPattern)
            {
                FsmBb.transform.rotation = Quaternion.LookRotation(direction);
                FsmBb.bCanPattern = true;
                FsmBb.OnAttack().Forget();
            }
        }
        else
        {
            //target이 없으면 Idle로 돌아간다.
            FsmBb.ResearchTarget();
            // if (Fsm.blackboard.target == null)
            // { 
            //     Fsm.ChangeEnemyState(Fsm.idleState);
            // }
        }
    }

    public override void Exit()
    {
        EnemyPathfinder.instance.ColliderReEnable(Fsm.blackboard.capsuleCol);
    }
}
