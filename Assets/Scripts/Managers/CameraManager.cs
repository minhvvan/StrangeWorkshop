using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera _mainCamera;
    private CinemachineBrain _brain;
    private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();
    private Transform _originTarget;
    
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
            Debug.Log($"카메라 '{activeVirtualCamera}'의 Follow 타겟이 '{followTarget.name}'으로 변경되었습니다.");
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