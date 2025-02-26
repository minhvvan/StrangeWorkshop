using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(IHoldableObjectParent parent)
    {
        if (!HasHoldableObject())
        {
            if (parent.HasHoldableObject())
            {
                parent.GiveHoldableObject(this);
            }
        }
        else
        {
            if (parent.HasHoldableObject())
            {
                if (GetHoldableObject().Acceptable(parent.GetHoldableObject()))
                {
                    parent.ClearHoldableObject();
                }
            }
            
            if (!parent.HasHoldableObject())
            {
                GiveHoldableObject(parent);
                TakeOffPlayerGlove(parent);
            }
        }
    }
}
