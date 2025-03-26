using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBotInteractionAlternate : BaseAction
{
    private LootBotInputHandler _inputHandler;
    private LootBotBlackBoard _lootBotBlackBoard;
    
    public override bool RegistAction()
    {
        _lootBotBlackBoard = GetComponent<LootBotBlackBoard>();
        _inputHandler = GetComponent<LootBotInputHandler>();

        _inputHandler.OnInteractAlternate += Interact;
        
        return true;
    }

    public override void UnregistAction()
    {
        _inputHandler.OnInteractAlternate -= Interact;
    }
    
    private void Interact()
    {
        _lootBotBlackBoard._selectedInteractable.InteractAlternate(_lootBotBlackBoard.lootBot);
    }
}
