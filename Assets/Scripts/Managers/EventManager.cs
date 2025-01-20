using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using VContainer;

public class EventManager : Singleton<EventManager>
{
    [Inject] private DataManager _dataManager;
    private readonly Dictionary<string, BaseEventSO> _eventDictionary = new();
    
    public async UniTask InitializeEvents()
    {
        //TODO: 사용할 Event Load
        var gameStateEvent = await _dataManager.LoadDataAsync<GameStateEventSO>(Addresses.Events.Game.STATE_CHANGED);
        RegisterEvent(Addresses.Events.Game.STATE_CHANGED, gameStateEvent);
        
        // 웨이브 이벤트
        // var waveEvent = await _dataManager.LoadDataAsync<WaveEventSO>(Addresses.Events.Game.WAVE_START);
        // RegisterEvent(Addresses.Events.Game.WAVE_START, waveEvent);
    }
    
    private void RegisterEvent(string key, BaseEventSO eventSO)
    {
        _eventDictionary.TryAdd(key, eventSO);
    }
    
    public T GetEvent<T>(string key) where T : BaseEventSO
    {
        if (_eventDictionary.TryGetValue(key, out var eventSO))
        {
            return eventSO as T;
        }
        
        return null;
    }
}
