using System;
using System.Collections.Generic;
using UnityEngine;

public class LootBotInputHandler: BaseInputHandler
{
    [SerializeField] private List<BaseAction> _actions = new List<BaseAction>();
    public Action OnInteract;
    public Action OnInteractAlternate;
    
    public override void OnControlStart()
    {
        foreach (var action in _actions)
        {
            action.RegistAction();
        }
    }

    public override void OnControlEnd()
    {
        base.OnControlEnd();
        
        foreach (var action in _actions)
        {
            action.UnregistAction();
        }
    }

    public override void ProcessInput(InputData input)
    {
        base.ProcessInput(input);
        
        // 1) 이동 입력
        Horizontal = input.moveInput.X;
        Vertical   = input.moveInput.Y;

        MovementInput = new Vector2(Horizontal, Vertical).normalized;
        IsWalking = MovementInput.magnitude > 0.1f;

        if (input.interactPressed)
        {
            OnInteract?.Invoke();
        }
        
        if (input.interactAlternatePressed)
        {
            OnInteractAlternate?.Invoke();
        }
    }
}
