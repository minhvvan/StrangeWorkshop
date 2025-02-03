using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ProcessCounter : BaseCounter
{
    public HoldableObjectSO burnSO;
    public ProgressBar progressBar;
    
    [NonSerialized] public float burnTime = 5f;
    [NonSerialized] public ProcessRecipeSO currentRecipe;
    [NonSerialized] public bool isWork = false;
    
    StateMachine _stateMachine;
    
    [NonSerialized] public ProcessCounter_NoneState _noneState;
    [NonSerialized] public ProcessCounter_ProcessingState _processingState;
    [NonSerialized] public ProcessCounter_OverState _overState;
    
    
    void Awake()
    {
        InitState();
    }
    
    private void Update()
    {
        _stateMachine.Update();
    }

    public override void Interact(SampleCharacterController player)
    {
        if (!HasHoldableObject())
        {
            if (player.HasHoldableObject())
            {
                //플레이어가 가지고 있는 HoldableObject로 가공품을 만들 수 있지 검사
                currentRecipe = RecipeManager.Instance.FindProcessRecipe(player.GetHoldableObject());
                if (currentRecipe.IsUnityNull()) return;
                player.GiveHoldableObject(this);
            }
        }
        else
        {
            if (!player.HasHoldableObject())
            {
                GiveHoldableObject(player);
                player.TakeoffGlove();
                if (!HasHoldableObject())
                {
                    SetState(_noneState);
                    isWork = false;
                    currentRecipe = null;
                }
            }
        }
    }

    // 레시피가 존재하면 상호작용
    public override void InteractAlternate(SampleCharacterController player)
    {
        if (!currentRecipe.IsUnityNull())
        {
            isWork = true;
        }
    }
    
    private void InitState()
    {
        _stateMachine = new StateMachine();

        _noneState = new ProcessCounter_NoneState(this);
        _processingState = new ProcessCounter_ProcessingState(this);
        _overState = new ProcessCounter_OverState(this);
        
        _stateMachine.ChangeState(_noneState);
    }
    
    public void SetState(IState newState)
    {
        _stateMachine.ChangeState(newState);
    }
}
