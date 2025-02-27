

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class UIAnimationUtility
{
    public static void PopupShow(RectTransform rect, float duration = 0.3f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        rect.localScale = Vector3.zero;
        rect.gameObject.SetActive(true);

        sequence.Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.OutBack)).OnComplete(() => callback?.Invoke());
    }
    
    public static void PopupHide(RectTransform rect, float duration = 0.3f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        sequence.Append(rect.DOScale(Vector3.zero, duration).SetEase(Ease.InBack)).OnComplete(() =>
        {
            callback?.Invoke();
            rect.gameObject.SetActive(false);
        });
    }

    public static void SlideInLeft(RectTransform rect,  float duration = 0.5f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        var endPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(-Screen.width, endPos.y);
        rect.gameObject.SetActive(true);
        
        sequence.Append(rect.DOAnchorPos(endPos, duration).SetEase(Ease.OutQuad)).OnComplete(() => callback?.Invoke());
    }
    
    public static void SlideInRight(RectTransform rect, float duration = 0.5f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        var endPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(Screen.width, endPos.y);
        rect.gameObject.SetActive(true);
        sequence.Append(rect.DOAnchorPos(endPos, duration).SetEase(Ease.OutQuad)).OnComplete(()=>callback?.Invoke());
    }

    public static void SlideOutRight(RectTransform rect, float duration = 0.5f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        var endPos = new Vector2(Screen.width, rect.anchoredPosition.y);
        sequence.Append(rect.DOAnchorPos(endPos, duration).SetEase(Ease.InQuad)).OnComplete(() =>
        {
            callback?.Invoke();
            rect.gameObject.SetActive(false);
        });
    }
    
    public static void SlideOutLeft(RectTransform rect, float duration = 0.3f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        var endPos = new Vector2(-Screen.width, rect.anchoredPosition.y);
        sequence.Append(rect.DOAnchorPos(endPos, duration).SetEase(Ease.InQuad)).OnComplete(() =>
        {
            callback?.Invoke();
            rect.gameObject.SetActive(false);
        });
    }
    
    public static void SlideOutDown(RectTransform rect, float duration = 0.3f, Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        var endPos = new Vector2(rect.anchoredPosition.x, -Screen.height);
        sequence.Append(rect.DOAnchorPos(endPos, duration).SetEase(Ease.InQuad)).OnComplete(() =>
        {
            callback?.Invoke();
            rect.gameObject.SetActive(false);
        });
    }

    // 페이드 애니메이션
    public static void FadeIn(CanvasGroup group, float duration = 0.3f, Action callback = null)
    {
        group.alpha = 0;
        group.gameObject.SetActive(true);
        group.DOFade(1, duration).OnComplete(() => callback?.Invoke());
    }

    public static void FadeOut(CanvasGroup group, float duration = 0.3f, Action callback = null)
    {
        group.DOFade(0, duration).OnComplete(() =>
        {
            callback?.Invoke();
            group.gameObject.SetActive(false);
        });
    }

    public static void MoveSmoothly(RectTransform rect, Vector2 endPos, float duration = 0.3f,
        Action callback = null)
    {
        rect.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);
        
        sequence.Append(rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutQuad)).OnComplete(() => callback?.Invoke());
    }
}
