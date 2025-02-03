using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarrierUI : MonoBehaviour
{
    [Header("UIElements")] 
    [SerializeField] private Image _barrierImage;
    [SerializeField] private Image _alertIcon;
    
    [Header("Effects")]
    [SerializeField] private float blinkDuration = 0.5f;  // 깜빡임 한번의 지속시간
    [SerializeField] private float blinkInterval = 0.2f;  // 깜빡임 사이의 간격
    [SerializeField] private int loopCount = 3;  // -1은 무한반복

    private Sequence blinkSequence;
    
    public void UpdateHealthColor(float percentage)
    {
        _barrierImage.color = GetHealthColor(percentage);
        if (percentage <= 0.33f)
        {
            StartBlinking();
        }
    }
    
    void StartBlinking()
    {
        blinkSequence ??= DOTween.Sequence();
        if (blinkSequence.playedOnce) return;
        
        _alertIcon.gameObject.SetActive(true);

        blinkSequence
            .Append(_alertIcon.DOFade(0f, blinkDuration))
            .AppendInterval(blinkInterval)
            .Append(_alertIcon.DOFade(1f, blinkDuration))
            .AppendInterval(blinkInterval)
            .SetLoops(loopCount)
            .AppendCallback(() => _alertIcon.gameObject.SetActive(false));
    }

    private Color GetHealthColor(float percentage)
    {
        if (percentage > 0.66f) return Color.green;
        if (percentage > 0.33f) return Color.yellow;
        if (percentage > 0f) return Color.red;
        return Color.gray;
    }
    
    void OnDestroy()
    {
        // 트윈 정리
        DOTween.Kill(_alertIcon);
    }
}
