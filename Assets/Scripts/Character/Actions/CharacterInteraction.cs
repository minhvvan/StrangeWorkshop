using System;
using UnityEngine;


/// <summary>
/// 상호작용을 위한 클래스
/// OnInteract에 이벤트를 등록하여 사용
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterInteraction : BaseAction
{
    CharacterController _controller;

    [SerializeField] float playerInteractDistance = 1f;
    [SerializeField] LayerMask playerInteractLayerMask;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _controller.AddAction(this);
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
        bool interactHit = Physics.Raycast(transform.position, transform.forward + (Vector3.down * (transform.position.y / 2)), out RaycastHit interactObject, playerInteractDistance, playerInteractLayerMask);
        
        //디버깅
        Debug.DrawRay(transform.position, transform.forward + (Vector3.down * transform.position.y) * playerInteractDistance, Color.red, 1f);

        if (interactHit)
        {
            Interact(interactObject.collider.gameObject);
        }
        else
        {
            Debug.Log("No interactable object");
        }
    }

    void Interact(GameObject interactObject)
    {
        Debug.Log("Interacting with " + interactObject.name);
    }
}