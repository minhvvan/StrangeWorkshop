using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LootBot : MonoBehaviour, IInteractAgent
{
    private LootBotInputHandler _inputHandler;
    private LootBotBlackBoard _blackBoard;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        _inputHandler = GetComponent<LootBotInputHandler>();
        _blackBoard = GetComponent<LootBotBlackBoard>();
        _blackBoard.OnBotPowerDown += HandlePowerDown;
    }

    private async void Start()
    {
        _cts = new CancellationTokenSource();
        
        _blackBoard.canMove = false;
        var animator = _blackBoard.animator;
        
        animator.Play("PowerUp");
        await UniTask.Yield();
        
        int layerIndex = 0;
        await UniTask.WaitUntil(() =>!animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("PowerUp") ||
                                animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 1 || 
                                _cts.IsCancellationRequested);
        
        _blackBoard.canMove = true;
    }
    
    private void Update()
    {
        if (Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit interactObject, _blackBoard.interactionDistance, _blackBoard.interactLayerMask))
        {
            if (interactObject.transform.TryGetComponent(out IInteractable interactable))
            {
                if (_blackBoard._selectedInteractable == interactable) return;
                
                _blackBoard._selectedInteractable?.GetGameObject().GetComponent<SelectObjectVisual>().Hide();
                _blackBoard._selectedInteractable = interactable;
                _blackBoard._selectedInteractable.GetGameObject().GetComponent<SelectObjectVisual>().Show();
            }
            else
            {
                ResetSelectedInteractable();
            }
        }
        else
        {
            ResetSelectedInteractable();
        }
    }

    private void ResetSelectedInteractable()
    {
        _blackBoard._selectedInteractable?.GetGameObject().GetComponent<SelectObjectVisual>().Hide();
        _blackBoard._selectedInteractable = null;
    }

    private async void HandlePowerDown()
    {
        _cts = new CancellationTokenSource();
        
        _blackBoard.OnBotPowerDown -= HandlePowerDown;
        _blackBoard.rigidbody.velocity = Vector3.zero;
        _blackBoard.rigidbody.angularVelocity = Vector3.zero;
        _blackBoard.animator.Play("PowerDown");
        _blackBoard.canMove = false;

        await UniTask.Yield();

        int layerIndex = 0;
        while (_blackBoard.animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1 && !_cts.IsCancellationRequested)
        {
            await UniTask.Yield();
        }

        ShutdownLootBot();
    }

    private void ShutdownLootBot()
    {
        Destroy(gameObject);
        CameraManager.Instance.ResetFollowTarget();
        InputManager.Instance.ReturnToPlayerControl();
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Gold"))
        {
            //TODO: 재화 수집(Gold 오브젝트에서 값 가져와야 함)
            _blackBoard.AddGold(10);
            Destroy(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SuccessReturn()
    {
        InGameDataManager.Instance.EarnGold(_blackBoard.stats.CurrentGold);
        ShutdownLootBot();
    }
}
