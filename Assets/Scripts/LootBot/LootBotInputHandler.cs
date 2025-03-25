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
        foreach (var action in _actions)
        {
            action.UnregistAction();
        }
    }

    public override void ProcessInput(InputData input)
    {
        base.ProcessInput(input);

        if (input.interactAlternatePressed)
        {
            //TODO 임시로 전환
            InputManager.Instance.ReturnToPlayerControl();
        }
    }
}
