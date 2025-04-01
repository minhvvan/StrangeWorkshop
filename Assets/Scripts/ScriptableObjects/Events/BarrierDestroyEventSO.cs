using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrierDestroyEvent", menuName = "SO/Event/BarrierDestroy")]
public class BarrierDestroyEventSO : BaseEventSO
{
    private event Action<int> _onEventRaised;
    
    public void Raise(int idx) => _onEventRaised?.Invoke(idx);
    public void AddListener(Action<int> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<int> listener) => _onEventRaised -= listener;
}