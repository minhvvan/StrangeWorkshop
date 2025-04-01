using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class BarrierCounterUI : MonoBehaviour
{
    [SerializeField] private Slider barrierHealthSlider;
    [SerializeField] private Slider energySlider;
    
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text energyText;
    
    [SerializeField] private float animationDuration = 0.2f;
 
    private Sequence _barrierHealthSequence;
    private Sequence _energySequence;

    private float _maxHealth;
    private float _maxEnergy;

    public void InitMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
    }
    
    public void InitMaxEnergy(float maxEnergy)
    {
        _maxEnergy = maxEnergy;
    }

    public void SetBarrierHealth(float newHealth)
    {
        if (!_barrierHealthSequence.IsUnityNull())
        {
            _barrierHealthSequence.Kill();
        }
    
        int.TryParse(healthText.text.Split(' ')[0], out var currentDisplayedHealth);
    
        _barrierHealthSequence = DOTween.Sequence();
    
        _barrierHealthSequence.Append(DOTween.To(() => barrierHealthSlider.value, 
            value => barrierHealthSlider.value = value, newHealth / _maxHealth, animationDuration));
    
        _barrierHealthSequence.Join(DOTween.To(() => currentDisplayedHealth, 
            health => healthText.text = $"{health} <color=#b3bedb> / {_maxHealth}</color>", 
            (int)newHealth, animationDuration));
    }
    
    public void SetEnergy(float newEnergy)
    {
        if (!_energySequence.IsUnityNull())
        {
            _energySequence.Kill();
        }
    
        int.TryParse(energyText.text.Split(' ')[0], out var currentDisplayedEnergy);
    
        _energySequence = DOTween.Sequence();
    
        _energySequence.Append(DOTween.To(() => energySlider.value, 
            value => energySlider.value = value, newEnergy / _maxEnergy, animationDuration));
    
        _energySequence.Join(DOTween.To(() => currentDisplayedEnergy, 
            energy => healthText.text = $"{energy} <color=#b3bedb> / {_maxEnergy}</color>", 
            (int)newEnergy, animationDuration));
    }
}