using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RadialSlider : MonoBehaviour
{
    [SerializeField] float animationDuration = 0.5f;
    
    private Image _image;
    private float _value;
    
    private Tween _sliderTween;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _value = 1f;
    }

    public void SetValue(float newValue)
    {
        if (!_sliderTween.IsUnityNull())
        {
            _sliderTween.Kill();
        }
        
        _sliderTween = DOTween.To(()=>_image.fillAmount,value=>_image.fillAmount=value, newValue, animationDuration);
    }

    private void OnDestroy()
    {
        if (!_sliderTween.IsUnityNull())
        {
            _sliderTween.Kill();
        }
    }
}
