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
    public int LifeCount { get; private set; }

    [Header("Events")] 
    private InGameUIController _inGameUIController;
    private BarrierDamagedEventSO _damagedEventSO;
    private BarrierDestroyEventSO _destroyEventSO;
    public Action OnBarrierDestroyed;

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
        LifeCount = Constants.LIFE_COUNT;
        for (var i = 0; i < Barriers.Count; i++)
        {
            Barriers[i].SetBarrierIndex(i);
            Barriers[i].InitHealth(healthPerBarrier);
        }
        
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
    }

    private void OnBarrierDamaged(float damage)
    {
        //TODO: 피격 처리
    }

    private void OnBarrierDestroy(int index)
    {
        LifeCount--;
        OnBarrierDestroyed?.Invoke();
        
        if (LifeCount <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}