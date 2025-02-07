using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SampleCharacterController))]
public class CharacterMovement : BaseAction
{
    // 착지 여부 및 수직속도
    private bool isGrounded;
    private float verticalVel;

    private float _dashEffDuration = 0.2f;
    private float _dashEffTimer = 0f;
    
    // CapsuleCast에 사용할 임의 값 (실 게임에 맞게 조정)
    [SerializeField] private float capsuleHeight = 1.8f;
    [SerializeField] private float capsuleRadius = 0.5f;
    
    // 필요한 컴포넌트
    private SampleCharacterController _controller;

    void Awake()
    {
        // 같은 오브젝트(또는 자식)에 있는 SampleCharacterController 가져오기
        _controller = GetComponent<SampleCharacterController>();
        // 이 액션을 등록
        _controller.AddAction(this);
        
    }

    public override bool RegistAction()
    {
        if (_controller.inputHandler == null) return false;

        // 이동은 OnActions 이벤트에 연결
        _controller.inputHandler.OnActions += MoveCharacter;
        return true;
    }

    public override void UnregistAction()
    {
        if (_controller.inputHandler == null) return;
        _controller.inputHandler.OnActions -= MoveCharacter;
    }

    /// <summary>
    /// 입력값으로 이동/회전 처리 (이동속도 = walkSpeed 고정)
    /// </summary>
    void MoveCharacter()
    {
        // 대쉬 중이면 이동 로직 스킵
        if (_controller.isDashing) return;

        var inputHandler = _controller.inputHandler;
        if (inputHandler == null) return;

        // 1) 바닥 체크 및 수직 속도 갱신
        HandleGravity();

        // 2) 이동 방향(수평) 계산 - 항상 walkSpeed 사용
        Vector3 inputDir = new Vector3(inputHandler.Horizontal, 0f, inputHandler.Vertical);
        if (inputDir.sqrMagnitude < 0.01f) // 데드존 임계치 설정
            inputDir = Vector3.zero;

        // 입력 크기를 0~1 범위로 제한(clamp)
        float inputMagnitude = Mathf.Clamp(inputDir.magnitude, 0f, 1f);

        // 정규화한 방향 벡터에 제한된 입력 크기를 곱해서 최종 이동 벡터 계산
        Vector3 moveDir = inputDir.normalized * _controller.walkSpeed * inputMagnitude;

        // (선택) 애니메이션 블렌딩을 위한 속도
        float speed = inputDir.sqrMagnitude; // 0 ~ 1 범위
        _controller.anim.SetFloat(
            "Blend",
            speed,
            (speed > _controller.allowPlayerRotation) ? _controller.StartAnimTime : _controller.StopAnimTime,
            Time.deltaTime
        );

        Vector3 capsuleStart = _controller.rb.position + Vector3.up * capsuleRadius;
        Vector3 capsuleEnd   = _controller.rb.position + Vector3.up * (capsuleHeight - capsuleRadius);
        
        // 3) 정면 장애물 체크(간단: 1m Raycast)
        RaycastHit hit;
        bool isBlocked = Physics.CapsuleCast(
            capsuleStart,
            capsuleEnd,
            capsuleRadius,
            moveDir.normalized,
            out hit,
            1.0f
        );
        
        // 4) 최종 velocity 결정
        //    - 기본적으로 수직 속도(verticalVel) 포함
        //    - 장애물이 없고, moveDir이 0이 아니면 수평 속도 적용
        Vector3 finalVelocity = new Vector3(0f, verticalVel, 0f);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 horizontalVel;
            
            if (isBlocked)
            {
                // 벽에 부딪힌 경우: 표면 노멀을 기준으로 moveDir을 평면투영하여 슬라이딩
                Vector3 slideDir = Vector3.ProjectOnPlane(moveDir, hit.normal);
                
                horizontalVel = slideDir;
            }
            else
            {
                // 평소엔 입력 방향 그대로
                horizontalVel = moveDir;
            }
            // 최종 속도에 x,z 반영
            finalVelocity.x = horizontalVel.x;
            finalVelocity.z = horizontalVel.z;

            // 이동 방향으로 회전
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            Quaternion smoothedRot = Quaternion.Slerp(_controller.rb.rotation, targetRot, _controller.desiredRotationSpeed);
            _controller.rb.MoveRotation(smoothedRot);
        }

        // Rigidbody에 최종 속도 세팅
        _controller.rb.velocity = finalVelocity;
        
        // 먼지나는 effect 실행
        _dashEffTimer += Time.deltaTime;
        if (_dashEffTimer >= _dashEffDuration && _controller.rb.velocity != Vector3.zero)
        {
            VFXManager.Instance.TriggerVFX(VFXType.PLAYERMOVE, transform.position, size: new Vector3(0.5f, 0.5f, 0.5f));
            _dashEffTimer = 0f;
        }
    }

    /// <summary>
    /// 레이캐스트로 착지를 확인하고, 수직 속도를 갱신한다.
    /// Rigidbody.useGravity = false 상태에서만 유효.
    /// </summary>
    void HandleGravity()
    {
        // 1.5m 아래 바닥이 있는지 Raycast
        Vector3 rayStart = _controller.rb.position + Vector3.up;
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 1.5f))
        {
            // 바닥임
            isGrounded = true;
            verticalVel = 0f;
        }
        else
        {
            // 공중임 -> 수동 중력 가속
            isGrounded = false;
            verticalVel -= 9.81f * Time.deltaTime;
        }
    }
}
