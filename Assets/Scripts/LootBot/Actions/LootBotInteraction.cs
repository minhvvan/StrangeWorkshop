using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBotInteraction : BaseAction
{
    private LootBotInputHandler _inputHandler;
    private LootBotBlackBoard _lootBotBlackBoard;
    
    public override bool RegistAction()
    {
        _lootBotBlackBoard = GetComponent<LootBotBlackBoard>();
        _inputHandler = GetComponent<LootBotInputHandler>();

        _inputHandler.OnInteract += Interact;
        
        return true;
    }

    public override void UnregistAction()
    {
        _inputHandler.OnInteract -= Interact;
    }
    
    private void Interact()
    {
        _lootBotBlackBoard._selectedInteractable?.Interact(_lootBotBlackBoard.lootBot);
    }
}
