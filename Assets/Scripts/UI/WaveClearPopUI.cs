using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WaveClearPopUI : MonoBehaviour
{
    private RectTransform rect;
    private Vector3 originPos;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originPos = rect.anchoredPosition;
    }

    private void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        RunUI();
    }

    public void RunUI()
    {
        canvasGroup.alpha = 1;
        rect.anchoredPosition = originPos;
        rect.DOAnchorPos(rect.anchoredPosition + new Vector2(-400f,0), 0.5f)
            .SetEase(Ease.InFlash);
        DOVirtual.DelayedCall(3f, () =>
        {
            canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear);
            rect.DOAnchorPos(originPos, 1f)
                .SetEase(Ease.OutFlash).OnComplete(() => gameObject.SetActive(false));
        });
    }

    void OnDestroy()
    {
        DOTween.Kill(rect);
    }
}
