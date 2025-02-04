using System.Collections;
using System.Collections.Generic;
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
        EnemyPathfinder.instance.enemyInCounter.Add(blackboard.capsuleCol);
        EnemyPathfinder.instance.ColliderSet(blackboard.capsuleCol);
        blackboard.SetMaxHp();
        blackboard.SetPattern();
        blackboard.SetPathfinder();
        blackboard.ResearchTarget();
    }

    private void Update()
    {
        _fsm.Update();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(2f);
        }
    }
    
    //사용 시, 이 객체에게 데미지를 가합니다.
    public void TakeDamage(float damage)
    {
        blackboard.ChangeMatColor(blackboard.matObject, blackboard.enemyStatus.hp -= damage);
        if (blackboard.enemyStatus.hp <= 0)
        {
            blackboard.cts?.Cancel();
            blackboard.rScts?.Cancel();
            blackboard.StopTracking();
            blackboard.AnimDead();
            Destroy(gameObject, 3f);
        }
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
        Gizmos.DrawWireSphere(blackboard.transform.position + new Vector3(0,1f,0), blackboard.enemyStatus.attackRange);
    }

    public void OnDestroy()
    {
        EnemyPathfinder.instance.enemyInCounter.Remove(blackboard.capsuleCol);
        if (EnemyPathfinder.instance.ignoreColliders.Contains(blackboard.capsuleCol))
        {
            EnemyPathfinder.instance.ignoreColliders.Remove(blackboard.capsuleCol);
        }
    }
}
