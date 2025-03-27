using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteCount : BaseCounter
{
    public override void Interact(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (parent.HasHoldableObject())
            {
                parent.ClearHoldableObject();
            
                TakeOffPlayerGlove(parent);
            }
        }
    }
}
