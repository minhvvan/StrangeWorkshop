using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialCounterVisual : MonoBehaviour
{
    // MaterialCounter 열리는 애니메이션
    private static readonly int OpenClose = Animator.StringToHash("OpenClose");
    
    [SerializeField] private MaterialCounter materialCounter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        materialCounter.OnPlayerGrabbedObject += MaterialCounterOnPlayerGrabbedObject;
    }

    private void MaterialCounterOnPlayerGrabbedObject(object sender, EventArgs e)
    {
        animator.SetTrigger(OpenClose);
    }
}
