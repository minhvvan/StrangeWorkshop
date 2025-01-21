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
    [NonSerialized] public Animator anim;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public CapsuleCollider capsuleCol;
    public EnemyStatus enemyStatus;
    public Transform target;
    public RaycastHit hit;

    public string layerName = "Water";
    public bool bDetectBarrier = false;
    public bool bCanAttack = false;
    
    public void InitBlackboard()
    {
        anim = GetComponent<Animator>();
        
        //부딫혔다고 빙빙 돌지않게.
        rb = GetComponent<Rigidbody>();
        rb.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
        
        capsuleCol = GetComponent<CapsuleCollider>();
    }
    
    public void SetTarget(Transform targetData)
    {
        target = targetData;
    }
}
