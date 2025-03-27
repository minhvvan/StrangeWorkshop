using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


[
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(CapsuleCollider)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(EnemyFsm)),
    RequireComponent(typeof(BlackboardEnemy)),
    RequireComponent(typeof(NavMeshAgent)),
]

//Enemy. 적의 주체를 담당하는 클래스.
public class Enemy : MonoBehaviour, IDamageable
{
    private EnemyFsm _fsm;
    private IBlackboardEnemy _blackboardEnemy;
    
    public BlackboardEnemy blackboard;
    
    private void Awake()
    {
        _fsm = GetComponent<EnemyFsm>();
        _blackboardEnemy = GetComponent<IBlackboardEnemy>();
        _blackboardEnemy.InitBlackboard();
        blackboard = _blackboardEnemy as BlackboardEnemy;
    }

    private void Start()
    {
        _fsm.InitStates();
        // EnemyPathfinder.instance.enemyInCounter.Add(blackboard.capsuleCol);
        // EnemyPathfinder.instance.ColliderSet(blackboard.capsuleCol);
        blackboard.SetModifyStat();
        blackboard.SetTypeSetting();
        blackboard.SetPattern();
        blackboard.ResearchTarget();
        blackboard.SetPathfinder();
        
        UpdateRigidbody();
    }

    private void Update()
    {
        _fsm.Update();
        //입력 시, 필드에 존재하는 모든 enemy에게 2 데미지를 가합니다.
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(2f);
        }
    }
    
    ///사용 시, 이 객체에게 데미지를 가합니다.
    public void TakeDamage(float damage)
    {
        
        //적 방어력에 따른 데미지 감소
        float dmgOperation = damage * (10f / (10f + blackboard.enemyStatus.armor));

        //보스인지에 따라 피격에 따른 색상변경 적용 판별
        if (blackboard.thisBoss == BlackboardEnemy.IsBoss.NONE)
        {
            blackboard.ChangeMatColor(blackboard.matObject, blackboard.enemyStatus.hp -= dmgOperation);
        }
        else
        {
            blackboard.enemyStatus.hp -= dmgOperation;
        }
        
        if (blackboard.enemyStatus.hp <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        blackboard.gameObject.layer = LayerMask.NameToLayer("Default");
        blackboard.rb.isKinematic = true;
        blackboard.cts?.Cancel();
        blackboard.autoResearchCts?.Cancel();
        blackboard.StopTracking();
        blackboard.DestroyPattern();
        blackboard.AnimDead();
        if (blackboard.thisBoss == BlackboardEnemy.IsBoss.BOSS) blackboard.OnBossEnd();
        VFXManager.Instance.TriggerVFX(VFXType.ENEMYDEATH, transform.position);
        // turret이 타겟팅하지 않도록 설정
        gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(gameObject, 3f);
    }

    public void DropTrophy()
    {
        //TODO: 처치시 전리품 드랍. 다양한 아이템을 떨굴 수 있게 만들기.
    }
    
    //RayCast 시각화
    public void OnDrawGizmos()
    {
        switch (blackboard.bDetectBarrier)
        {
            case false : Gizmos.color = Color.red;
                break;
            case true : Gizmos.color = Color.blue;
                break;
        }

        //Gizmos.DrawLine(blackboard.transform.position, blackboard.hitNearPoint);
        Gizmos.DrawWireSphere(blackboard.transform.position + new Vector3(0,1f,0), blackboard.enemyStatus.attackRange);
    }

    public void OnDestroy()
    {
        // EnemyPathfinder.instance.enemyInCounter.Remove(blackboard.capsuleCol);
        // if (EnemyPathfinder.instance.ignoreColliders.Contains(blackboard.capsuleCol))
        // {
        //     EnemyPathfinder.instance.ignoreColliders.Remove(blackboard.capsuleCol);
        // }

        //언 카운트
        EnemySpawner.Instance.enemyCountList.Remove(gameObject);
    }

    public async UniTask UpdateRigidbody()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(40));
            blackboard.rb.velocity += Vector3.zero;
        }
    }
}
