using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class WaveUIController : MonoBehaviour, IGameUI
{
    public static WaveUIController instance;

    public delegate void OnUI();
    public OnUI _onUI;
    
    [SerializeField] private WaveClearPopUI _waveClearPop;
    [SerializeField] private WaveAlertPopUI _waveAlertPop;
    
    private void Awake()
    { 
        instance = this;
    }

    public void SetWaveUIController(OnUI onUI)
    {
        CleanUp();
        _onUI = onUI;
        Initialize();
        ShowUI();
    }

    public void SetPreparationTime(float duration)
    {
        // popupDuration = duration;
    }
    
    public void OnWaveClearPopup()
    {
        _waveClearPop.gameObject.SetActive(true);
        _waveClearPop.RunUI();
    }

    public void OnWaveAlertPopup(float popupDuration)
    {
        _waveAlertPop.gameObject.SetActive(true);
        _waveAlertPop.RunUI(popupDuration);
    }
    
    public void ShowUI()
    {
        _onUI?.Invoke();
    }

    public void HideUI()
    {
        
    }
    
    public void Initialize()
    {
        
    }
    
    public void CleanUp()
    {
        
    }
}
