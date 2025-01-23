using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteCount : BaseCounter
{
    public override void Interact(SampleCharacterController player)
    {
        if (player.HasHoldableObject())
        {
            player.ClearHoldableObject();
            player.TakeoffGlove();
        }
    }
}
