using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    [Tooltip("기본 이동 속도")]
    public float walkSpeed = 5f;
    [Tooltip("달리기(Shift) 배속")]
    public float sprintMultiplier = 1.5f;

    [Header("Dash Settings")]
    [Tooltip("대쉬 최대 속도")]
    public float dashMaxSpeed = 15f;
    [Tooltip("대쉬 전체 지속 시간(초)")]
    public float dashDuration = 0.5f;
    [Tooltip("대쉬 쿨타임(초)")]
    public float dashCooldown = 1f;

    private bool isDashing = false;    // 대쉬 중인지 여부
    private float nextDashTime = 0f;   // 다음 대쉬 가능 시점(Time.time과 비교)

    void Update()
    {
        HandleMovement();
        HandleDash();
    }

    /// <summary>
    /// W/A/S/D(또는 방향키) + Shift로 이동/달리기
    /// </summary>
    void HandleMovement()
    {
        // 대쉬 중에는 일반 이동 로직 무시
        if (isDashing) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 입력 벡터를 정규화해서 대각선 이동 시에도 일정 속도 유지
        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        // Shift키로 달리기
        float currentSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // transform 기준 전후좌우로 이동
        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    /// 스페이스바로 대쉬 (Lerp를 사용해 빨라졌다가 느려지게)
    /// </summary>
    void HandleDash()
    {
        // 이미 대쉬 중이거나, 쿨타임이 안 됐다면 무시
        if (isDashing) return;
        if (Time.time < nextDashTime) return;

        // 스페이스바 누르면 대쉬 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DashRoutine());
        }
    }

    /// <summary>
    /// 대쉬를 일정 시간 동안 속도가 빨라졌다가(가속) 다시 느려지는(감속) 형태로 구현
    /// </summary>
    IEnumerator DashRoutine()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown; // 쿨타임 갱신

        float elapsed = 0f;         // 대쉬 경과 시간
        float halfTime = dashDuration * 0.5f;  // 대쉬 시간 절반

        // 대쉬 방향 (입력값 기준)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 dashDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 만약 입력이 없으면 정면(또는 원하는 방향)으로 대쉬하도록 처리
        if (dashDirection.magnitude < 0.1f)
        {
            dashDirection = transform.forward;
        }

        // 대쉬: 첫 절반(가속), 두 번째 절반(감속)
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed < halfTime)
            {
                // 0 ~ dashMaxSpeed 로 가속
                float t = elapsed / halfTime; 
                float currentDashSpeed = Mathf.Lerp(0f, dashMaxSpeed, t);
                transform.Translate(dashDirection * currentDashSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                // dashMaxSpeed ~ 0 으로 감속
                float t = (elapsed - halfTime) / halfTime;
                float currentDashSpeed = Mathf.Lerp(dashMaxSpeed, 0f, t);
                transform.Translate(dashDirection * currentDashSpeed * Time.deltaTime, Space.Self);
            }

            yield return null;
        }

        isDashing = false;
    }
}
