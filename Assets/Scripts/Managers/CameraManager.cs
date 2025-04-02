using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    private Camera _mainCamera;
    private CinemachineBrain _brain;
    private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();
    private Transform _originTarget;
    
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private float _normalHeight = 21f;
    [SerializeField] private float _elevatedHeight = 50f;
    
    protected override void Awake()
    {
        base.Awake();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeVirtualCameras();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;
        _brain = _mainCamera?.GetComponent<CinemachineBrain>();
        InitializeVirtualCameras();
    }
    
    private Tween _cameraTween;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TransitionCameraHeight(_elevatedHeight);
        }
        
        if (Input.GetKeyUp(KeyCode.Z))
        {
            TransitionCameraHeight(_normalHeight);
        }
    }
    
    private void TransitionCameraHeight(float targetHeight)
    {
        ICinemachineCamera activeVirtualCamera = _brain.ActiveVirtualCamera;
        if (activeVirtualCamera is CinemachineVirtualCamera virtualCamera)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            
            if (transposer != null)
            {
                _cameraTween?.Kill();
                
                _cameraTween = DOTween.To(
                    () => transposer.m_FollowOffset.y,
                    (y) => 
                    {
                        Vector3 offset = transposer.m_FollowOffset;
                        offset.y = y;
                        transposer.m_FollowOffset = offset;
                    },
                    targetHeight,
                    _transitionDuration
                ).SetEase(Ease.OutQuad);
            }
        }
    }

    private void InitializeVirtualCameras()
    {
        _virtualCameras.Clear();
        _virtualCameras.AddRange(FindObjectsOfType<CinemachineVirtualCamera>());
    }

    public void SetFollowTarget(GameObject followTarget)
    {
        // 활성화된 가상 카메라 찾기
        ICinemachineCamera activeVirtualCamera = _brain.ActiveVirtualCamera;
        
        if (activeVirtualCamera != null)
        {
            _originTarget = activeVirtualCamera.Follow;
            activeVirtualCamera.Follow = followTarget.transform;
        }
        else
        {
            Debug.LogWarning("활성화된 가상 카메라를 찾을 수 없습니다.");
        }
    }

    public void SetLookAtTarget(GameObject lookAtTarget)
    {
        ICinemachineCamera activeVirtualCamera = _brain.ActiveVirtualCamera;

        if (activeVirtualCamera != null)
        {
            activeVirtualCamera.LookAt = lookAtTarget.transform;
            Debug.Log($"카메라 '{activeVirtualCamera}'의 LookAt 타겟이 '{lookAtTarget.name}'으로 변경되었습니다.");
        }
    }

    public void ResetFollowTarget()
    {
        ICinemachineCamera activeVirtualCamera = _brain.ActiveVirtualCamera;

        if (activeVirtualCamera != null)
        {
            activeVirtualCamera.Follow = _originTarget;
        }
    }
}