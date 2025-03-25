
using System;
using UnityEngine;

public abstract class BaseInputHandler : MonoBehaviour, IControllable
{

    public Vector2 MovementInput { get; protected set; } 
    
    public bool IsRunning { get; protected set; }
    public float Horizontal { get; protected set; }
    public float Vertical { get; protected set; } 
    public bool IsWalking { get; protected set; } 
    
    public bool IsDashing { get; protected set; }
    public bool IsGrounded { get; protected set; } 

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public LayerMask groundLayer;

    
    //추가 input 필요하다면 상속받은 클래스에서 추가로 등록하여 사용
    [NonSerialized] public Action<InputData> OnActions; 


    protected virtual void Update()
    {
        IsGrounded = CheckIfGrounded();
    }

    //나중에 발아래에 BoxCollider를 하나 넣어서 체크해도 좋아보임
    private bool CheckIfGrounded()
    {
        // 발 밑에서 Raycast를 쏘아서 지면을 체크
        bool ret = false;
        if(Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer))
        {
            ret = true;
        }

        if(Physics.Raycast(transform.position+ new Vector3(0.5f, 0, 0), Vector3.down, groundCheckDistance, groundLayer))
        {
            ret = true;
        }

        if(Physics.Raycast(transform.position+ new Vector3(-0.5f, 0, 0), Vector3.down, groundCheckDistance, groundLayer))
        {
            ret = true;
        }

        // Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
        // Debug.DrawRay(transform.position+ new Vector3(0.5f, 0, 0), Vector3.down * groundCheckDistance, Color.red);
        // Debug.DrawRay(transform.position+ new Vector3(-0.5f, 0, 0), Vector3.down * groundCheckDistance, Color.red);

        return ret;
    }

    public virtual void OnControlStart()
    {
    }

    public virtual void OnControlEnd()
    {
    }

    public virtual void ProcessInput(InputData input)
    {
        OnActions?.Invoke(input);
    }
}
