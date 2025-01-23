using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    
    [SerializeField] private HoldableObjectSO holdableObjectSO;
    
    public override void Interact(SampleCharacterController player)
    {
        if (!player.HasHoldableObject())
        {
            HoldableObject.SpawnHoldableObject(holdableObjectSO, player);
            
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            player.TakeoffGlove();
        }
    }
}
