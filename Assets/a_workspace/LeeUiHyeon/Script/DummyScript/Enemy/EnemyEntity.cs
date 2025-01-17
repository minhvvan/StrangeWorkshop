using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(EnemyFsm)),
]

public abstract class EnemyEntity : MonoBehaviour
{
    protected EnemyFsm fsm;
    protected Rigidbody rigidbody;
    
    protected virtual void Awake()
    {
        fsm = GetComponent<EnemyFsm>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ;
    }

    protected virtual void Start()
    {
        fsm.InitStates();
    }

    protected virtual void Update()
    {
        fsm.Update();
    }
}
