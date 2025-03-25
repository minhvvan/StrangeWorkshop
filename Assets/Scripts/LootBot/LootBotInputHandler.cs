using System;
using System.Collections.Generic;
using UnityEngine;

public class LootBotInputHandler: BaseInputHandler
{
    [SerializeField] private List<BaseAction> _actions = new List<BaseAction>();

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

        if (input.interactAlternatePressed)
        {
            //TODO 임시로 전환 -> 전환 규칙 만들어야 함
            InputManager.Instance.ReturnToPlayerControl();
        }
    }
}
