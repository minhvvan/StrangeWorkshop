using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [Header("Data")] 
    private BarrierStatSO _barrierStat;
    private float _totalHealth;
    private float _maxHealth;
    private bool _isDestroyed;
    public List<Barrier> Barriers { get; private set; }
    public float TotalHeath => _totalHealth;
    public float MaxHealth => _maxHealth;

    [Header("Events")] 
    private BarrierDestroyEventSO _destroyEventSO;
    private BarrierDamagedEventSO _damagedEventSO;
    public Action<Barrier> OnBarrierDamagedAction;

    private async void Awake()
    {
        //Init
        _barrierStat = await DataManager.Instance.LoadDataAsync<BarrierStatSO>(Addresses.Data.Barrier.STAT);
        if (_barrierStat)
        {
            _maxHealth = _barrierStat.totalHP;
            _totalHealth = _barrierStat.totalHP;
        }
        
        //Event
        _damagedEventSO = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        _destroyEventSO = await DataManager.Instance.LoadDataAsync<BarrierDestroyEventSO>(Addresses.Events.Barrier.BARRIER_DESTROY);
        _damagedEventSO.AddListener(OnBarrierDamaged);
        _destroyEventSO.AddListener(OnBarrierDestroyed);

        //Find All Barrier
        Barriers = GetComponentsInChildren<Barrier>().ToList();
        Barriers.Sort((a, b) =>  b.BarrierType.CompareTo(a.BarrierType));
        
        //Init Barrier HP
        var barrierHP = _barrierStat.totalHP / Barriers.Count;
        foreach (var barrier in Barriers)
        {
            barrier.InitHealth(barrierHP);
        }

        //Sync UI
        var barrierUIController = UIManager.Instance.GetUI<BarrierUIController>(UIType.MinimapUI);
        barrierUIController.SetBarrierController(this);
    }

    private void OnBarrierDamaged(Barrier barrier, float damage)
    {
        _totalHealth -= damage;
        OnBarrierDamagedAction?.Invoke(barrier);
    }

    private void OnBarrierDestroyed(Barrier barrier)
    {
        Barriers.Remove(barrier);
        if (Barriers.Count == 0)
        {
            GameManager.Instance.RequestChangeGameState(GameState.GameOver);
        }
    }
}