using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    public Image image;
    private Slider _slider;
    private Camera _mainCamera;
    private Camera _uiCamera;
    private Canvas _canvas;
    private Transform _owner;
    
    void Awake()
    {
        _slider = GetComponent<Slider>();
        ResetBar();
        _canvas = GetComponentInParent<Canvas>();
        _mainCamera = Camera.main;
        _uiCamera = _canvas.worldCamera;
    }
    
    public void SetBar(float value)
    {
        _slider.maxValue = value;
    }

    public void ResetBar()
    {
        _slider.value = 0;
    }

    public void UpdateProgressBar(float value)
    {
        _slider.value = value;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetColorLerp(Color color)
    {
        image.DOColor(color, 0.5f);
    }
}
