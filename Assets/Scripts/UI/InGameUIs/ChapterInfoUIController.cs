using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterInfoUIController : MonoBehaviour, IGameUI
{
    private RectTransform _root;
    
    [SerializeField] private RectTransform _waveProgress;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private RectTransform _gold;
    [SerializeField] private TMP_Text _goldText;

    [Header("wave progress")]
    private int _currentWave = 1;
    
    [Header("gold")] 
    private int _currentGold = 0;

    void Awake()
    {
        _root = GetComponent<RectTransform>();
    }
    
    public void ShowUI()
    {
        UIAnimationUtility.SlideInRight(_root);
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutRight(_root, callback:()=>{_root.anchoredPosition = originalPos;});
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    public void UpdateWaveProgress()
    {
        _currentWave++;
        _waveText.text = $"{_currentWave}  /  10";
    }
    
    public void UpdateGold(int gold)
    {
        _currentGold = gold;
        _goldText.text = $"{_currentGold}";
    }
}
