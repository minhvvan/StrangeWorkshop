using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 상호작용을 위한 클래스
/// OnInteract에 이벤트를 등록하여 사용
/// </summary>
[RequireComponent(typeof(SampleCharacterController))]
public class CharacterInteraction : BaseAction
{
    SampleCharacterController _controller;
    
    // equiement UI
    [NonSerialized] public Action<HoldableObject> OnHoldObjectAction;
    private InGameUIController _inGameUIController;
    
    void Awake()
    {
        _controller = GetComponent<SampleCharacterController>();

        _controller.AddAction(this);

        InitUI();
    }
    
    public override bool RegistAction()
    {
        if(_controller.inputHandler == null) return false;

        _controller.inputHandler.OnInteract += HandleInteraction; //단일 이벤트에 연결

        return true;
    }

    public override void UnregistAction()
    {
        if(_controller.inputHandler == null) return;

        _controller.inputHandler.OnInteract -= HandleInteraction;       
    }

    void HandleInteraction()
    {
        if (!_controller.GetSelectedInteractableObject().IsUnityNull())
        {
            _controller.GetSelectedInteractableObject().Interact(_controller);
        }

        OnHoldObjectAction?.Invoke(_controller.GetHoldableObject());
    }

    private async void InitUI()
    {
        // await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        // _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        // _inGameUIController.RegisterGameUI(this);
    }
}