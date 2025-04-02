using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class Barrier : MonoBehaviour, IDamageable
{
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
    
    private GameObject _barrierEff;

    public bool Destroyed { get; private set; }

    private void Awake()
    {
        _barrierCounterUIRect = _barrierCounterUI.GetComponent<RectTransform>();
    }

    async void Start()
    {
#if UNITY_EDITOR
        await UniTask.WaitUntil(() => GameBootstrapper.IsInitialized);
#endif
        Destroyed = false;
        _damagedEventSo = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        _destroyEventSo = await DataManager.Instance.LoadDataAsync<BarrierDestroyEventSO>(Addresses.Events.Barrier.BARRIER_DESTROYED);
    }

    public void TakeDamage(float damage)
    {
        if (Destroyed) return;
        
        _damagedEventSo.Raise(damage);
        
        _currentHealth -= damage;
        
        if (_currentHealth <= 0)
        {
            Destroyed = true;
            _currentHealth = 0;
            _destroyEventSo.Raise(_barrierIndex);
        }
        
        UpdateUI();
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

    public bool Repair(float amount)
    {
        //현재 최대체력인지 확인
        if (Mathf.Approximately(_currentHealth, _maxHealth)) return false;
        
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth,0, _maxHealth);

        UpdateUI();

        return true;
    }

    private void UpdateUI()
    {
        _barrierCounterUI.SetBarrierHealth(_currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        
        UIAnimationUtility.PopupShow(_barrierCounterUIRect, duration:0.1f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        UIAnimationUtility.PopupHide(_barrierCounterUIRect, duration:0.1f);
    }
}