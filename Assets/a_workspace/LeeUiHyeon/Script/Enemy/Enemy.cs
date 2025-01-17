using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(EnemyFsm)),
    RequireComponent(typeof(BlackboardEnemy))
]
public class Enemy : MonoBehaviour, IDamageable
{
    public BlackboardEnemy blackboardEnemy;

    private EnemyFsm fsm;
    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    
    void Awake()
    {
        fsm = GetComponent<EnemyFsm>();
        
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        
        blackboardEnemy = GetComponent<BlackboardEnemy>();
    }

    void Start()
    {
        fsm.InitStates();
    }

    void Update()
    {
        fsm.Update();
    }
    
    public void TakeDamage(float damage)
    {
        blackboardEnemy.enemyStatus.hp -= damage;
    }
}
