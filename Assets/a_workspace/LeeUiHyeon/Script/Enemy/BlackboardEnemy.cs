using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlackboardEnemy
{
    void InitBlackboard();
}

//블랙보드 담당 클래스
public class BlackboardEnemy : MonoBehaviour, IBlackboardEnemy
{
    [NonSerialized] public Animator animator;
    [NonSerialized] public Rigidbody rigidbody;
    [NonSerialized] public CapsuleCollider capsuleCollider;
    public EnemyStatus enemyStatus;
    public Transform target;
    public RaycastHit hit;

    public string layerName = "Water";
    public bool bDetectBarrier = false;
    public bool bCanAttack = false;
    
    public void InitBlackboard()
    {
        Debug.Log("InitBlackboard");
        
        animator = GetComponent<Animator>();
        
        //부딫혔다고 빙빙 돌지않게.
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
}
