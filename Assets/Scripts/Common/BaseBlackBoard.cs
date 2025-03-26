using System;
using UnityEngine;

public class BaseBlackBoard: MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
}