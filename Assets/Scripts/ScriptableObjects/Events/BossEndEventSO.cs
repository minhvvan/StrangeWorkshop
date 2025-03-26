using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///보스같은 특정된 목적을 해결 했을 때 사용하는 이벤트.
[CreateAssetMenu(fileName = "BossEndEvent", menuName = "SO/Event/BossEndEvent")]
public class BossEndEventSO : BaseEventSO
{
    private event Action _onEventRaised;
    
    public void Raise() => _onEventRaised?.Invoke();
    public void AddListener(Action listener) => _onEventRaised += listener;
    public void RemoveListener(Action listener) => _onEventRaised -= listener;
}
