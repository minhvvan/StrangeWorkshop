using System;
using UnityEngine;

public class BaseBlackBoard: MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
}