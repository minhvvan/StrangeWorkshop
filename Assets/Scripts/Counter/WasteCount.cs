using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteCount : BaseCounter
{
    public override void Interact(IHoldableObjectParent parent)
    {
        if (parent.HasHoldableObject())
        {
            parent.ClearHoldableObject();
            
            TakeOffPlayerGlove(parent);
        }
    }
}
