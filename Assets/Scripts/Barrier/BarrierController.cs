using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Managers;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] float healthPerBarrier;
    private BarrierStatSO _barrierStat;
    public List<Barrier> Barriers { get; private set; }

    private int _destroyedBarrierCount;

    [Header("Events")] 
    private InGameUIController _inGameUIController;
    private BarrierDamagedEventSO _damagedEventSO;
    private BarrierDestroyEventSO _destroyEventSO;
    public Action<float> OnBarrierHealthChangedAction;

    private async void Awake()
    {
#if UNITY_EDITOR
        await UniTask.WaitUntil(() => GameBootstrapper.IsInitialized);
#endif
        //Event
        _damagedEventSO = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        _destroyEventSO = await DataManager.Instance.LoadDataAsync<BarrierDestroyEventSO>(Addresses.Events.Barrier.BARRIER_DESTROYED);
        _damagedEventSO.AddListener(OnBarrierDamaged);
        _destroyEventSO.AddListener(OnBarrierDestroy);
        Barriers = GetComponentsInChildren<Barrier>().ToList();
        
        //Init
        _destroyedBarrierCount = 0;
        for (var i = 0; i < Barriers.Count; i++)
        {
            Barriers[i].SetBarrierIndex(i);
            Barriers[i].InitHealth(healthPerBarrier);
        }
    }

    private void OnBarrierDamaged(float damage)
    {
        //TODO: 피격 처리
    }

    private void OnBarrierDestroy(int index)
    {
        //TODO: Barriers[i] 파괴 UI Update
        _destroyedBarrierCount++;
        
        if (_destroyedBarrierCount >= 3)
        {
            GameManager.Instance.GameOver();
        }
    }
}