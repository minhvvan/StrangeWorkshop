using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterInfoUIController : MonoBehaviour, IGameUI
{
    [SerializeField] RectTransform _waveProgress;
    [SerializeField] RectTransform _gold;
    [SerializeField] RectTransform _achievements;

    private Dictionary<string, RectTransform> _rects;

    [Header("wave progress")]
    private int _currentWave = 1;
    private TMP_Text _waveText;

    [Header("gold")] 
    private int _currentGold = 0;
    private TMP_Text _goldText;

    void Awake()
    {
        _rects = new Dictionary<string, RectTransform>();
        _rects.Add("WaveProgress", _waveProgress);
        _rects.Add("Gold", _gold);
        _rects.Add("Achievements", _achievements);
        
        _waveText = _waveProgress.GetComponent<TMP_Text>();
        _goldText = _gold.GetComponent<TMP_Text>();
    }
    
    public void ShowUI()
    {
        foreach (var rect in _rects.Values)
        {
            UIAnimationUtility.SlideInLeft(rect);
        }
    }

    public void HideUI()
    {
        foreach (var rect in _rects.Values)
        {
            Vector2 originalPos = rect.anchoredPosition;
            UIAnimationUtility.SlideOutLeft(rect, callback: (() => { rect.anchoredPosition = originalPos; }));
        }
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
    
    public void UpdateGold(int goldChangeAmount)
    {
        _currentGold += goldChangeAmount;
        _goldText.text = $"{_currentGold}";
    }

    public void UpdateAchievements()
    {
        
    }
}
