

using DG.Tweening;
using UnityEngine;

public static class UIAnimationUtility
{
    public static void PopupShow(RectTransform rect, float duration = 0.3f)
    {
        rect.DOKill();
        
        rect.localScale = Vector3.zero;
        rect.gameObject.SetActive(true);
        rect.DOScale(Vector3.one, duration).
            SetEase(Ease.OutBack);
    }
    
    public static void PopupHide(RectTransform rect, float duration = 0.3f)
    {
        rect.DOKill();

        rect.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).OnComplete(() =>
        {
            rect.gameObject.SetActive(false);
        });
    }

    public static void SlideInLeft(RectTransform rect,  float duration = 0.5f)
    {
        rect.anchoredPosition = new Vector2(-Screen.width, 0);
        rect.gameObject.SetActive(true);
        rect.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutQuad);
    }
    
    public static void SlideInRight(RectTransform rect, float duration = 0.5f)
    {
        var endPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(Screen.width, endPos.y);
        rect.gameObject.SetActive(true);
        rect.DOAnchorPos(endPos, duration).SetEase(Ease.OutQuad);
    }

    public static void SlideOutRight(RectTransform rect, float duration = 0.5f)
    {
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);

        var startPos = rect.anchoredPosition;
        sequence.Append(rect.DOAnchorPos(new Vector2(Screen.width, startPos.y), duration).SetEase(Ease.InQuad))
            .AppendCallback(() => rect.gameObject.SetActive(false));
    }
    
    public static void SlideOutLeft(RectTransform rect, float duration = 0.3f)
    {
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);

        var startPos = rect.anchoredPosition;
        sequence.Append(rect.DOAnchorPos(new Vector2(-Screen.width, startPos.y), duration).SetEase(Ease.InQuad))
            .AppendCallback(() => rect.gameObject.SetActive(false));
    }
    
    public static void SlideOutDown(RectTransform rect, float duration = 0.3f)
    {
        var sequence = DOTween.Sequence();
        sequence.SetLink(rect.gameObject);

        var startPos = rect.anchoredPosition;
        sequence.Append(rect.DOAnchorPos(new Vector2(startPos.x, -Screen.height), duration).SetEase(Ease.InQuad))
            .AppendCallback(() => rect.gameObject.SetActive(false));
    }

    // 페이드 애니메이션
    public static void FadeIn(CanvasGroup group, float duration = 0.3f)
    {
        group.alpha = 0;
        group.gameObject.SetActive(true);
        group.DOFade(1, duration);
    }

    public static void FadeOut(CanvasGroup group, float duration = 0.3f)
    {
        var sequence = DOTween.Sequence();
        sequence.SetLink(group.gameObject);

        sequence.Append(group.DOFade(0, duration))
            .AppendCallback(() => group.gameObject.SetActive(false));
    }
}
