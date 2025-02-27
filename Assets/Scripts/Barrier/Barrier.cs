using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class Barrier : MonoBehaviour, IDamageable
{
    //TODO: 삭제===========================
    [Header("Data")]
    private bool _isDestroyed;
    private BarrierStat _barrierStat = new();
    public BarrierStat BarrierStat => _barrierStat;
    //===========================

    
    private GameObject _barrierEff;
    
    [Header("Event")] 
    private BarrierDamagedEventSO _damagedEventSo;

    [Header("Components")] 
    [SerializeField] private MeshRenderer _damagedFilter;
    [SerializeField] private List<Material> _damagedMats;

    private void Awake()
    {
        _barrierStat.id = gameObject.GetInstanceID();
    }

    async void Start()
    {
#if UNITY_EDITOR
        await UniTask.WaitUntil(() => GameBootstrapper.IsInitialized);
#endif
        _damagedEventSo = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        
        float x = GetComponent<BoxCollider>().size.x / 2f;
        _barrierEff = VFXManager.Instance.TriggerVFX(VFXType.BARRIERSHIELD, transform, rotation: Quaternion.Euler(-90f, 0f, 0f),
            size: new Vector3(x, 1f, 10f), returnAutomatically:false);
    }

    public void TakeDamage(float damage)
    {
        _damagedEventSo.Raise(damage);
    }
}

//TODO: 삭제===========================
[Serializable]
public class BarrierStat
{
    public int id;
    public float health;
    public float maxHealth;
}
//===========================