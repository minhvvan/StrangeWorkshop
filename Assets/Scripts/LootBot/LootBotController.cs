using System;
using UnityEngine;

public class LootBotController : MonoBehaviour, IInteractable
{
    private LootBotInputHandler _lootBotInputHandler;
    
    private void Awake()
    {
        _lootBotInputHandler = GetComponent<LootBotInputHandler>();
    }

    public void Interact(IHoldableObjectParent parent = null)
    {
        InputManager.Instance.SwitchControl(_lootBotInputHandler);
    }

    public void InteractAlternate(IHoldableObjectParent parent = null)
    {
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
