using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveAlertPopUI : MonoBehaviour
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text text;
    private Vector3 originPos;
    private float duration;

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
        
    }

    public async UniTask RunUI(float time)
    {
        duration = time;
        canvasGroup.alpha = 1;
        rect.anchoredPosition = originPos;
        
        rect.DOAnchorPos(rect.anchoredPosition + new Vector2(-400f,0), 0.5f)
            .SetEase(Ease.InFlash);
        while (duration > 0)
        {
            text.text = "now comming.."+duration.ToString();
            duration--;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
        canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear);
        rect.DOAnchorPos(originPos, 1f)
            .SetEase(Ease.OutFlash).OnComplete(() => gameObject.SetActive(false));
    }

    void OnDestroy()
    {
        DOTween.Kill(rect);
    }
}
