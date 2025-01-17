using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlackboardEnemy
{
    void InitBlackboard();
}

public class BlackboardEnemy : MonoBehaviour, IBlackboardEnemy
{
    [NonSerialized] public Animator animator;
    [NonSerialized] public Rigidbody rigidbody;
    [NonSerialized] public CapsuleCollider capsuleCollider;
    public EnemyStatus enemyStatus;
    public Transform target;
    public void InitBlackboard()
    {
        Debug.Log("InitBlackboard");
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
}
