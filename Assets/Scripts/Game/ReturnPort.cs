using UnityEngine;

public class ReturnPort : MonoBehaviour, IInteractable
{
    public void Interact(IInteractAgent agent = null)
    {
        //TODO: 재화 추가 및 로봇 삭제
        
    }

    public void InteractAlternate(IInteractAgent agent = null)
    {
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
