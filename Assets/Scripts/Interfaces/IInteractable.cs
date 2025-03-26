using UnityEngine;

public interface IInteractable
{
     public void Interact(IInteractAgent agent = null);
     public void InteractAlternate(IInteractAgent agent = null);

     public GameObject GetGameObject();
}

public interface IInteractAgent
{
     public GameObject GetGameObject();
}