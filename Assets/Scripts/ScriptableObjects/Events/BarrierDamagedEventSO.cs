using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrierDamagedEvent", menuName = "SO/Event/BarrierDamaged")]
public class BarrierDamagedEventSO  : BaseEventSO
{
    private event Action<Barrier, float> _onEventRaised;
    
    public void Raise(Barrier barrier, float damage) => _onEventRaised?.Invoke(barrier, damage);
    public void AddListener(Action<Barrier, float> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<Barrier, float> listener) => _onEventRaised -= listener;
}