using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        //target이 있으면 실행.
        if (Fsm.blackboard.target != null)
        {
            BlackboardEnemy FsmBb = Fsm.blackboard;
            Vector3 targetPos = FsmBb.target.position;
            Vector3 currentPos = FsmBb.transform.position;
            Vector3 direction = targetPos - currentPos;
            //float distance = Vector3.Distance(targetPos, currentPos);
            
            //바라보는 방향 사거리 내 방벽과 닿았는지 확인.
            Physics.Raycast(FsmBb.transform.position,FsmBb.transform.forward, out FsmBb.hit, 
                FsmBb.enemyStatus.attackRange,1 << LayerMask.NameToLayer(FsmBb.layerName));
            
            //target과의 거리가 사거리보다 길면 Chase로 이동.
            //if (FsmBb.enemyStatus.attackRange < distance)
            if(FsmBb.hit.collider == null)
            {
                FsmBb.bDetectBarrier = false;
                Fsm.ChangeEnemyState(Fsm.chaseState);
                return;
            }
            
            //적 공격 상태를 유지한다.
            if (!FsmBb.bCanAttack)
            {
                FsmBb.transform.rotation = Quaternion.LookRotation(direction);
                FsmBb.bCanAttack = true;
                HoldAttack(FsmBb);
            }
        }
        else
        {
            //target이 없으면 Idle로 돌아간다.
            Fsm.ChangeEnemyState(Fsm.idleState);
        }
    }

    public override void Exit()
    {
        
    }

    public async void HoldAttack(BlackboardEnemy FsmBb)
    {
        Debug.Log("Keep Attack");
        
        //attackSpeed값에 비례하여 공격속도가 정해진다.
        await UniTask.Delay((int)(1000*FsmBb.enemyStatus.attackSpeed));
        FsmBb.bCanAttack = false;
    }
}
