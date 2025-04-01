using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class Barrier : MonoBehaviour, IDamageable
{
    private GameObject _barrierEff;
    
    [Header("Event")] 
    private BarrierDamagedEventSO _damagedEventSo;
    private BarrierDestroyEventSO _destroyEventSo;

    [Header("Components")] 
    [SerializeField] private MeshRenderer _damagedFilter;
    [SerializeField] private List<Material> _damagedMats;
    [SerializeField] private BarrierCounterUI _barrierCounterUI;

    private RectTransform _barrierCounterUIRect;
    
    private float _maxHealth;
    private float _currentHealth;
    private int _barrierIndex;
    private bool _destroyed;

    private void Awake()
    {
        _barrierCounterUIRect = _barrierCounterUI.GetComponent<RectTransform>();
    }

    async void Start()
    {
#if UNITY_EDITOR
        await UniTask.WaitUntil(() => GameBootstrapper.IsInitialized);
#endif
        _destroyed = false;
        _damagedEventSo = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
    }

    public void TakeDamage(float damage)
    {
        if (_destroyed) return;
        
        _damagedEventSo.Raise(damage);
        
        _currentHealth -= damage;
        UpdateUI();
        
        if (_currentHealth <= 0)
        {
            _destroyed = true;
            _currentHealth = 0;
            _destroyEventSo.Raise(_barrierIndex);
        }
        
        QuestManager.Instance.Notify(QuestType.ProtectBarrier, damage);
    }

    public void InitHealth(float health)
    {
        _maxHealth = health;
        _currentHealth = health;
        _barrierCounterUI.InitMaxHealth(_maxHealth);
        UpdateUI();
    }

    public void SetBarrierIndex(int i)
    {
        _barrierIndex = i;
    }

    public void Repair(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Max(_currentHealth, _maxHealth);

        UpdateUI();
    }

    private void UpdateUI()
    {
        _barrierCounterUI.SetBarrierHealth(_currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        UIAnimationUtility.PopupShow(_barrierCounterUIRect, duration:0.1f);
    }

    private void OnTriggerExit(Collider other)
    {
        UIAnimationUtility.PopupHide(_barrierCounterUIRect, duration:0.1f);
    }
}