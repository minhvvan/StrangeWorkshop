using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ProcessCounter_BaseMinigame : BaseState<ProcessCounter>
{
    public ProcessCounter_BaseMinigame(ProcessCounter controller) : base(controller) {}

    GameObject _minigame;
    const string CanvasTag = "MinigameCanvas";
    Canvas _minigameCanvas;
    
    
    public override void Enter()
    {
        _controller.progressBar.SetColor(Color.green);
        _controller.progressBar.gameObject.SetActive(false);
        CreateMinigame();
    }

    public override void UpdateState()
    {
    }

    public void LateUpdateState()
    {
        SetMinigamePosition();
    }

    public override void Exit()
    {
        if (_controller.GetHoldableObjectParent() is SampleCharacterController sampleCharacterController)
        {
            sampleCharacterController.SetState(sampleCharacterController.idleState);
        }
    }


    public void CreateMinigame()
    {
        if (GameObject.FindWithTag(CanvasTag) != null)
        {
            _minigameCanvas = GameObject.FindWithTag(CanvasTag).GetComponent<Canvas>();
            var minigamePrefab = _controller.minigamePrefab[Random.Range(0, _controller.minigamePrefab.Length)];

            _minigame = GameObject.Instantiate(minigamePrefab, _minigameCanvas.transform);
            MinigameBaseController minigameController = _minigame.GetComponent<MinigameBaseController>();
            minigameController.OnSuccess += Success;
            minigameController.OnFail += Fail;        
            
            SetMinigamePosition();
        }
        else
        {
            Debug.LogError("MinigameCanvas is not found");
        }
    }

    void SetMinigamePosition()
    {
        if(_minigame == null) return;
        if(_minigameCanvas == null) return;

        RectTransform canvasRect = _minigameCanvas.GetComponent<RectTransform>();
        Camera camera = Camera.main;
        Vector3 worldPosition = _controller.transform.position;
        Vector2 screenPosition = WorldToCanvasPosition(canvasRect, camera, worldPosition);
        Vector2 offset = new Vector2(0, 100);
        _minigame.transform.localPosition = screenPosition + offset;
    }

    public Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 worldPosition)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPosition); // 월드 -> 뷰포트 좌표 변환
        Vector2 canvasSize = canvas.sizeDelta; // 캔버스 크기 가져오기

        return new Vector2(
            (viewportPosition.x - 0.5f) * canvasSize.x, // 뷰포트 좌표를 캔버스 좌표로 변환
            (viewportPosition.y - 0.5f) * canvasSize.y
        );
    }

    public void Success()
    {
        _controller.ClearHoldableObject();
        //플레이어에게 직접 오브젝트를 넘겨주도록 구현
        HoldableObject.SpawnHoldableObject(_controller.currentRecipe.output, _controller.GetHoldableObjectParent(), _controller.GetHoldableObjectFollowTransform());
        _controller.SetState(_controller._noneState);
        
    }

    public void Fail()
    {
        //마지막에 상호작용하는 플레이어 변수 _controller.GetHoldableObjectParent()
        //우선적으로 돌려주자
        _controller.GiveHoldableObject(_controller.GetHoldableObjectParent());
        _controller.SetState(_controller._noneState);
    }
}

    