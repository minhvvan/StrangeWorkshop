using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClearUIController : BasePopupUI
{
    [SerializeField] RectTransform backgroundBar;
    [SerializeField] CustomTMPAnimaition tmpAnimation;

    [Header("Background Pattern")]
    [SerializeField] RectTransform backgroundPattern;
    [SerializeField] float backgroundPatternMoveSpeed = 200f;

    public Vector2 backgroundBarOriginPosition = new Vector2(-100, 0);

    public new async void ShowUI()
    {
        backgroundBar.anchoredPosition = backgroundBarOriginPosition;

        base.ShowUI();
        UIAnimationUtility.SlideInLeft(backgroundBar, 0.3f, null, true);
        await Task.Delay(300);
        tmpAnimation.PlayAnimation(CustomTMPAnimaition.TMPAnimationType.EntryAndJump);
        AnimateBackgroundPattern();

        await Task.Delay(2000);
        HideUI();
    }

    public new void HideUI()
    {
        tmpAnimation.PlayAnimation(CustomTMPAnimaition.TMPAnimationType.FadeOut, null, 0.1f);
        UIAnimationUtility.SlideOutRight(backgroundBar, 0.3f, () => {
            base.HideUI();
            UIManager.Instance.GetUI<ResultUIContorller>(UIType.ResultUI).ShowUI();
        }, true);
    }

    async void AnimateBackgroundPattern()
    {
        while(IsOpen)
        {
            backgroundPattern.anchoredPosition += (Vector2.right + (Vector2.down * Mathf.Tan(20 * Mathf.Deg2Rad))) * backgroundPatternMoveSpeed * Time.unscaledDeltaTime;

            float screenWidth = Screen.width;
            if(backgroundPattern.anchoredPosition.x > screenWidth)
            {
                backgroundPattern.anchoredPosition = Vector2.zero;
            }

            await Task.Yield();
        }
    }
}
