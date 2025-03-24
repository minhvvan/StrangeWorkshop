using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnEnemyEvent", menuName = "SO/Event/SpawnEnemyEvent")]
public class SpawnEnemyEventSO : BaseEventSO
{
    private event Action<List<EnemySpawnInfo>> _onEventRaised;
    
    public void Raise(List<EnemySpawnInfo> enemySpawnInfo) => _onEventRaised?.Invoke(enemySpawnInfo);
    public void AddListener(Action<List<EnemySpawnInfo>> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<List<EnemySpawnInfo>> listener) => _onEventRaised -= listener;
}