using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrierUIController : MonoBehaviour, IGameUI
{
    private BarrierController _barrierController;
    private RectTransform _root;

    [Header("UI Elements")]
    [SerializeField] private Slider _totalHealthBar;
    [SerializeField] private TMP_Text _currentHealthText;
    [SerializeField] private TMP_Text _maxHealthText;

    private void Awake()
    {
        _root = transform.GetComponent<RectTransform>();
    }

    public void SetBarrierController(BarrierController newBarrierController)
    {
        if (newBarrierController == null) return;
        CleanUp();
        
        _barrierController = newBarrierController;
        _barrierController.OnBarrierHealthChangedAction += UpdateBarrierTotalHealth;
        
        Initialize();
        ShowUI();
    }

    public void ShowUI()
    {
        UIAnimationUtility.SlideInLeft(_root);   
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_root, callback: () => _root.anchoredPosition = originalPos);
    }

    public void CleanUp()
    {
        //Controller 초기화
        _barrierController = null;
    }

    public void Initialize()
    {
        if (!_barrierController) return;
        _maxHealthText.text = $"{_barrierController.MaxHealth}";
        UpdateBarrierTotalHealth(_barrierController.MaxHealth);
    }
    
    private void UpdateBarrierTotalHealth(float totalHealth)
    {
        // 총 체력 UI 업데이트
        _currentHealthText.text = $"{_barrierController.TotalHeath}";
        _totalHealthBar.value = _barrierController.TotalHeath / _barrierController.MaxHealth;
    }
}