using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(SampleCharacterController player)
    {
        if (!HasHoldableObject())
        {
            if (player.HasHoldableObject())
            {
                player.GiveHoldableObject(this);
            }
        }
        else
        {
            if (player.HasHoldableObject())
            {
                if (GetHoldableObject().Acceptable(player.GetHoldableObject()))
                {
                    player.ClearHoldableObject();
                }
            }
            
            if (!player.HasHoldableObject())
            {
                GiveHoldableObject(player);
                player.TakeoffGlove();
            }
        }
    }
}
