using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrierDamagedEvent", menuName = "SO/Event/BarrierDamaged")]
public class BarrierDamagedEventSO  : BaseEventSO
{
    private event Action<float> _onEventRaised;
    
    public void Raise(float damage) => _onEventRaised?.Invoke(damage);
    public void AddListener(Action<float> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<float> listener) => _onEventRaised -= listener;
}