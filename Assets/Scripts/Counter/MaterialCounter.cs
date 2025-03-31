using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialCounter : BaseCounter
{
    [SerializeField] private HoldableObjectSO holdableObjectSO;

    private readonly float _rotateSpeed = 100f;

    void Start()
    {
        HoldableObject.SpawnHoldableObject(holdableObjectSO, this, null);
    }
    
    private void Update()
    {
        if (HasHoldableObject())
        {
            GetHoldableObject().transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    public override void Interact(IHoldableObjectParent parent)
    {
        if (!parent.HasHoldableObject() && HasHoldableObject())
        {
            GiveHoldableObject(parent);
            StartCoroutine(SpawnHoldableObject());
            TakeOffPlayerGlove(parent);
        }
    }

    IEnumerator SpawnHoldableObject()
    {
        yield return new WaitForSeconds(3f);
        HoldableObject.SpawnHoldableObject(holdableObjectSO, this, GetHoldableObjectFollowTransform());
    }
}
