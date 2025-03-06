using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveStartEvent", menuName = "SO/Event/WaveStartEvent")]
public class WaveStartEventSO : BaseEventSO
{
    private event Action<float> _onEventRaised;
    
    public void Raise(float time) => _onEventRaised?.Invoke(time);
    public void AddListener(Action<float>  listener) => _onEventRaised += listener;
    public void RemoveListener(Action<float>  listener) => _onEventRaised -= listener;
}