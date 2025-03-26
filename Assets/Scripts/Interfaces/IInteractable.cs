using UnityEngine;

public interface IInteractable
{
     public void Interact(IHoldableObjectParent parent = null);
     public void InteractAlternate(IHoldableObjectParent parent = null);

     public GameObject GetGameObject();
}