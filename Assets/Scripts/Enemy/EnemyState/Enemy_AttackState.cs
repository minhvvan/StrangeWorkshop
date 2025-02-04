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
            
            //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
            FsmBb.SearchNearTarget();
            
            //target과의 거리가 사거리보다 길면 Chase로 이동.
            if(FsmBb.targetCollider == null)
            {
                FsmBb.bCanAttack = false;
                FsmBb.cts?.Cancel();
                FsmBb.cts = new CancellationTokenSource();
                //공격이 끝나지 않았다면 잠시 대기.
                if (!FsmBb.bCanAttack)
                {
                    FsmBb.bDetectBarrier = false;
                    Fsm.ChangeEnemyState(Fsm.chaseState);
                    return;
                }
            }
            
            //적 공격 상태를 유지한다.
            if (!FsmBb.bCanAttack)
            {
                FsmBb.transform.rotation = Quaternion.LookRotation(direction);
                FsmBb.bCanAttack = true;
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
