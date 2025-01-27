using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(CapsuleCollider)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(EnemyFsm)),
    RequireComponent(typeof(BlackboardEnemy))
]

//Enemy. 적의 주체를 담당하는 클래스.
public class Enemy : MonoBehaviour, IDamageable
{
    private EnemyFsm _fsm;
    private IBlackboardEnemy _blackboardEnemy;
    public BlackboardEnemy blackboard;
    
    void Awake()
    {
        _fsm = GetComponent<EnemyFsm>();
        _blackboardEnemy = GetComponent<IBlackboardEnemy>();
        _blackboardEnemy.InitBlackboard();
        blackboard = _blackboardEnemy as BlackboardEnemy;
    }

    void Start()
    {
        _fsm.InitStates();
    }

    void Update()
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
        blackboard.enemyStatus.hp -= damage;
        if (blackboard.enemyStatus.hp <= 0)
        {
            blackboard.cts?.Cancel();
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
        Gizmos.DrawRay(
            blackboard.transform.position,
            blackboard.transform.forward * blackboard.enemyStatus.attackRange);
    }
}
