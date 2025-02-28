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
    private BarrierStatSO _barrierStat;
    private float _totalHealth;
    private float _maxHealth;
    private bool _isDestroyed = true;
    public List<Barrier> Barriers { get; private set; }
    public float TotalHeath => _totalHealth;
    public float MaxHealth => _maxHealth;


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
        //Init
        _barrierStat = await DataManager.Instance.LoadDataAsync<BarrierStatSO>(Addresses.Data.Barrier.STAT);
        if (_barrierStat)
        {
            _maxHealth = _barrierStat.totalHP;
            _totalHealth = _barrierStat.totalHP;
        }
        _isDestroyed = false;
        
        //Event
        _damagedEventSO = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        _destroyEventSO = await DataManager.Instance.LoadDataAsync<BarrierDestroyEventSO>(Addresses.Events.Barrier.BARRIER_DESTROYED);
        _damagedEventSO.AddListener(OnBarrierDamaged);
        _destroyEventSO.AddListener(OnBarrierDestroy);
        Barriers = GetComponentsInChildren<Barrier>().ToList();
        
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
    }

    private void OnBarrierDamaged(float damage)
    {
        if(_isDestroyed) return;

        _totalHealth -= damage;

        if (_totalHealth <= 0)
        {
            _totalHealth = 0;
            _isDestroyed = true;
            _destroyEventSO.Raise();
        }
        
        OnBarrierHealthChangedAction?.Invoke(_totalHealth);
    }

    private void OnBarrierDestroy()
    {
        GameManager.Instance.GameOver();
    }
}