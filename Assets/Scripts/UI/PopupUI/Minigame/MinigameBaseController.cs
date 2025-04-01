using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class MinigameBaseController : MonoBehaviour
{
    public Action OnSuccess;
    public Action OnFail;

     public enum MinigameState{
        None,
        Idle,
        Playing,
        Success,
        Fail,
        Translating,
    }
    public MinigameState minigameState = MinigameState.None;

    protected abstract void ChangeState(MinigameState newState);
    

    void OnEnable()
    {
        InitShow();
        ChangeState(MinigameState.Idle);
    }

    public void InitShow()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void Dispose()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}