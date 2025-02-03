using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrierDestroyEvent", menuName = "SO/Event/BarrierDestroy")]
public class BarrierDestroyEventSO : BaseEventSO
{
    private event Action<Barrier> _onEventRaised;
    
    public void Raise(Barrier barrier) => _onEventRaised?.Invoke(barrier);
    public void AddListener(Action<Barrier> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<Barrier> listener) => _onEventRaised -= listener;
}