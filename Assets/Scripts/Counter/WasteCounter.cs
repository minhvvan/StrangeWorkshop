using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteCounter : BaseCounter
{
    [SerializeField] Animator _animator;
    
    public override void Interact(IInteractAgent agent = null)
    {
        Debug.Log("Interact");
        
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (parent.HasHoldableObject())
            {
                //TODO: Anim
                Debug.Log("Anim Triggered");

                _animator.SetTrigger("Interact");
                
                parent.ClearHoldableObject();
            
                TakeOffPlayerGlove(parent);
            }
        }
    }
}
