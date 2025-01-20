using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class Player : MonoBehaviour, IHoldableObjectParent
{
    public static Player Instance { get; private set; }
    
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    
    [SerializeField] private Transform holdableObjectHoldPoint;
    private Transform gloveObject;
    private BaseCounter selectedCounter;
    private HoldableObject _holdableObject;
    
    
    private bool isWalking;
    private Vector3 lastInteractDir;
    
    
    void Awake()
    {
        if(Instance != null)
            Debug.Log("Error: More than one instance of Player");
        Instance = this;
    }
    
    void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            //selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            //selectedCounter.Interact(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleInteract();
    }

    void HandleInteract()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 movedir = new Vector3(inputVector.x, 0, inputVector.y);

        if (movedir != Vector3.zero)
        {
            lastInteractDir = movedir;
        }
        
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectdCounter(baseCounter);
                }
            }
            else
            {
                SetSelectdCounter(null);
            }
        }
        else
        {
            SetSelectdCounter(null);
        }
    }
    
    void HandleMove()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 movedir = new Vector3(inputVector.x, 0, inputVector.y);
        
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movedir, moveDistance);
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(movedir.x, 0, 0).normalized;
            canMove = movedir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                movedir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, movedir.z).normalized;
                canMove = movedir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    movedir = moveDirZ;
                }
            }
        }
        
        if (canMove)
        {
            transform.position += movedir * (Time.deltaTime * moveSpeed);   
        }
        isWalking = movedir != Vector3.zero;
        transform.forward = Vector3.Lerp(transform.forward, movedir, rotationSpeed * Time.deltaTime);
    }

    private void SetSelectdCounter(BaseCounter counter)
    { 
        selectedCounter = counter;
                    
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    public Transform GetHoldableObjectFollowTransform()
    {
        return holdableObjectHoldPoint;
    }

    public void SetHoldableObject(HoldableObject holdableObject)
    {
        this._holdableObject = holdableObject;
    }

    public void GiveHoldableObject(IHoldableObjectParent parent)
    {
        _holdableObject.SetHoldableObjectParent(parent);
        _holdableObject = null;
    }

    public HoldableObject GetHoldableObject()
    {
        return _holdableObject;
    }

    public void ClearHoldableObject()
    {
        Destroy(_holdableObject.gameObject);
        _holdableObject = null;
    }

    public bool HasHoldableObject()
    {
        return _holdableObject != null;
    }

    public bool CanSetHoldableObject()
    {
        return gloveObject != null;
    }

    public void WearGlove(Transform glove)
    {
        glove.parent = GetHoldableObjectFollowTransform();
        gloveObject = glove;
    }

    public void TakeoffGlove()
    {
        if (gloveObject != null)
            Destroy(gloveObject.gameObject);
        gloveObject = null;
    }
}
