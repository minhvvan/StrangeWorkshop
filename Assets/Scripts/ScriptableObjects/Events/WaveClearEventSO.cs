using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveClearEvent", menuName = "SO/Event/WaveClearEvent")]
public class WaveClearEventSO : BaseEventSO
{
    private event Action _onEventRaised;
    
    public void Raise() => _onEventRaised?.Invoke();
    public void AddListener(Action listener) => _onEventRaised += listener;
    public void RemoveListener(Action listener) => _onEventRaised -= listener;
}