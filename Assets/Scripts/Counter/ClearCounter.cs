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
                if (player.GetHoldableObject().GetHoldableObjectSO().objectType == HoldableObjectType.Bullet && GetHoldableObject().GetHoldableObjectSO().objectType == HoldableObjectType.Turret)
                {
                    player.ClearHoldableObject();
                    GetHoldableObject().GetComponent<Turret>().turretActions.Reload();
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
