using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandCompleteEvent", menuName = "SO/Event/CommandCompleteEvent")]
public class CommandCompleteEventSO : BaseEventSO
{
    private event Action _onCommandCompleted;
    
    public void Raise() => _onCommandCompleted?.Invoke();
    public void AddListener(Action listener) => _onCommandCompleted += listener;
    public void RemoveListener(Action listener) => _onCommandCompleted -= listener;
}
