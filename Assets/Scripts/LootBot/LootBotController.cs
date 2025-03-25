using System;
using UnityEngine;

public class LootBotController : MonoBehaviour, IInteractable
{
    private LootBotInputHandler _lootBotInputHandler;
    
    private void Awake()
    {
        _lootBotInputHandler = GetComponent<LootBotInputHandler>();
    }

    public void Interact()
    {
        InputManager.Instance.SwitchControl(_lootBotInputHandler);
    }
}
