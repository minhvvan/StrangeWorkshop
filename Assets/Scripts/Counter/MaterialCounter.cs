using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialCounter : BaseCounter
{
    [SerializeField] private HoldableObjectSO holdableObjectSO;
    
    public override void Interact(IHoldableObjectParent parent)
    {
        if (!parent.HasHoldableObject())
        {
            HoldableObject.SpawnHoldableObject(holdableObjectSO, parent);
            
            TakeOffPlayerGlove(parent);
        }
    }
}
