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
    [SerializeField] List<Image> lifeImages;
    [SerializeField] List<Sprite> lifeSprites;
    
    private void Awake()
    {
        _root = transform.GetComponent<RectTransform>();
    }

    public void SetBarrierController(BarrierController newBarrierController)
    {
        if (newBarrierController == null) return;
        CleanUp();
        
        _barrierController = newBarrierController;
        _barrierController.OnBarrierDestroyed += UpdateLife;
        
        Initialize();
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

        for (int i = 0; i < _barrierController.LifeCount; i++)
        {
            lifeImages[i].sprite = lifeSprites[0];
        }
    }
    
    private void UpdateLife()
    {
        lifeImages[_barrierController.LifeCount].sprite = lifeSprites[1];
    }
}