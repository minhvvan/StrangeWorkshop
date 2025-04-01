using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SampleCharacterController))]
public class CharacterDash : BaseAction
{
    [SerializeField] private LayerMask collisionLayerMask;

    private SampleCharacterController _controller;

    void Awake()
    {
        _controller = GetComponent<SampleCharacterController>();
        _controller.AddAction(this);
    }

    public override bool RegistAction()
    {
        if (_controller.inputHandler == null) return false;
        _controller.inputHandler.OnDash += Dash;
        return true;
    }

    public override void UnregistAction()
    {
        if (_controller.inputHandler == null) return;
        _controller.inputHandler.OnDash -= Dash;
    }

    void Dash()
    {
        if (!_controller.isDashing)
        {
            _controller.SetState(_controller.dashState);
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        //상태이상 활성화 시 종료
        if (!_controller.isMoveable) yield break;
        
        // 1) 대쉬 시작
        _controller.isDashing = true;
        _controller.rb.velocity = Vector3.zero; // 기존 속도 초기화

        // 총 대쉬 시간(초) = 가속 구간 + 감속 구간
        float totalDashTime = _controller.dashAccelTime + _controller.dashDecelTime;
        float timer = 0f;
        float dashEffDuration = 0.05f;
        float dashEffTimer = dashEffDuration;

        // 2) 대쉬 루프 (가속+감속을 하나의 while로 처리)
        while (timer < totalDashTime)
        {
            timer += Time.deltaTime;
            dashEffTimer += Time.deltaTime;

            // 현재 구간이 가속인지, 감속인지 판별
            float currentDashSpeed = 0f;
            if (timer < _controller.dashAccelTime)
            {
                // 가속 구간: (walkSpeed → dashSpeed)
                float t = timer / _controller.dashAccelTime;
                currentDashSpeed = Mathf.Lerp(_controller.walkSpeed, _controller.dashSpeed, t);
            }
            else
            {
                // 감속 구간: (dashSpeed → walkSpeed)
                float elapsedDecel = timer - _controller.dashAccelTime;
                float t = elapsedDecel / _controller.dashDecelTime;
                currentDashSpeed = Mathf.Lerp(_controller.dashSpeed, _controller.walkSpeed, t);
            }

            // 3) 현재 입력(수평) * 대쉬 속도로 이동 벡터 계산
            Vector3 inputDir = new Vector3(
                _controller.inputHandler.MovementInput.x,
                0f,
                _controller.inputHandler.MovementInput.y
            );
            Vector3 movement = inputDir.normalized * currentDashSpeed;

            // 4) 전방 충돌 체크 → 슬라이딩
            Vector3 moveDir = movement.normalized;
            bool isBlocked = Physics.Raycast(
                _controller.rb.position,
                moveDir,
                out RaycastHit hit,
                2.0f,
                collisionLayerMask
            );

            if (isBlocked)
            {
                // 벽이 있으면 슬라이딩
                Vector3 slideDir = Vector3.ProjectOnPlane(movement, hit.normal);

                if (slideDir.sqrMagnitude > 0.001f)
                {
                    // 슬라이딩 이동
                    _controller.rb.velocity = slideDir;

                    // 회전
                    Quaternion rot = Quaternion.LookRotation(slideDir.normalized);
                    _controller.rb.MoveRotation(rot);
                }
                else
                {
                    // 거의 수직으로 막혔으면 정지
                    _controller.rb.velocity = Vector3.zero;
                }
            }
            else
            {
                // 벽이 없으면 그대로 이동
                _controller.rb.velocity = movement;

                // 회전
                if (movement.sqrMagnitude > 0.001f)
                {
                    Quaternion rot = Quaternion.LookRotation(moveDir);
                    _controller.rb.MoveRotation(rot);
                }
            }
            
            // 먼지나는 이펙트 출현
            if (dashEffTimer >= dashEffDuration)
            {
                VFXManager.Instance.TriggerVFX(VFXType.PLAYERMOVE, transform.position);
                dashEffTimer = 0f;
            }
            yield return null; // 다음 프레임까지 대기
        }

        // 5) 대쉬 종료
        _controller.isDashing = false;
        _controller.rb.velocity = Vector3.zero;
    }
}
