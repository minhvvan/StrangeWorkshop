using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteCount : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasHoldableObject())
        {
            player.ClearHoldableObject();
            player.TakeoffGlove();
        }
    }
}
