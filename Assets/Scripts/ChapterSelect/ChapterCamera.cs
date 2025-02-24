using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ChapterCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private (Vector3 offset, float fov) _defaultValues;
    private Transform _defaultLookAt;
    private Transform _defaultFollowTarget;
    
    [Header("Focus Settings")]
    public float zoomFOV = 30f;                // 줌인 시 FOV
    public float defaultFOV = 60f;             // 기본 FOV
    public float transitionDuration = 1f;       // 전환 시간
    public Vector3 cameraOffset;               // 포커스 시 카메라 오프셋
    public Ease easeType = Ease.InOutQuad;    // DOTween 이징
    
    [Header("FOV Scaling")]
    public float baseScale = 30f;     // 기준이 되는 스케일
    public float minFOV = 40f;       // 최소 FOV
    public float maxFOV = 80f;       // 최대 FOV

    private void Awake()
    {
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _defaultLookAt = _virtualCamera.LookAt;
        _defaultFollowTarget = _virtualCamera.Follow;
    }

    private void SaveDefaultValues(Vector3 offset, float fov)
    {
        _defaultValues = (offset, fov);
    }
    
    public void FocusIn(GameObject target)
    {
        var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        SaveDefaultValues(transposer.m_FollowOffset, _virtualCamera.m_Lens.FieldOfView);
        
        // LookAt 타겟을 현재 선택된 오브젝트로 변경
        _virtualCamera.LookAt = target.transform;
        _virtualCamera.Follow = target.transform;
        
        float targetScale = target.transform.localScale.x;
        float adjustedFOV = zoomFOV * (targetScale / baseScale);
        adjustedFOV = Mathf.Clamp(adjustedFOV, minFOV, maxFOV);
        
        // 카메라 오프셋 트위닝
        DOTween.To(() => transposer.m_FollowOffset,
                x => transposer.m_FollowOffset = x,
                cameraOffset,
                transitionDuration)
            .SetEase(easeType);

        // FOV 트위닝
        DOTween.To(() => _virtualCamera.m_Lens.FieldOfView,
                x => _virtualCamera.m_Lens.FieldOfView = x,
                adjustedFOV,
                transitionDuration)
            .SetEase(easeType);
    }
    
    public void FocusOut()
    {
        var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        _virtualCamera.LookAt = _defaultLookAt;
        _virtualCamera.Follow = _defaultFollowTarget;
        
        // 카메라 오프셋을 원래 위치로
        DOTween.To(() => transposer.m_FollowOffset,
                x => transposer.m_FollowOffset = x,
                _defaultValues.offset,
                transitionDuration)
            .SetEase(easeType);

        // FOV를 원래 값으로
        DOTween.To(() => _virtualCamera.m_Lens.FieldOfView,
                x => _virtualCamera.m_Lens.FieldOfView = x,
                _defaultValues.fov,
                transitionDuration)
            .SetEase(easeType);
    }
}
