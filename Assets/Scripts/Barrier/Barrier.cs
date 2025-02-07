using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class Barrier : MonoBehaviour, IDamageable
{
    [Header("Data")]
    private bool _isDestroyed;
    private BarrierStat _barrierStat = new();
    public BarrierStat BarrierStat => _barrierStat;
    
    public BarrierType BarrierType;
    private DamagedColor _damagedColor;
    private GameObject _barrierEff;
    [SerializeField] private GridPosition gridPosition;
    public GridPosition GridPos => gridPosition;
    
    [Header("Event")] 
    private BarrierDestroyEventSO _destroyEventSO;
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
        _isDestroyed = false;
        _destroyEventSO = await DataManager.Instance.LoadDataAsync<BarrierDestroyEventSO>(Addresses.Events.Barrier.BARRIER_DESTROY);
        _damagedEventSo = await DataManager.Instance.LoadDataAsync<BarrierDamagedEventSO>(Addresses.Events.Barrier.BARRIER_DAMAGED);
        
        float x = GetComponent<BoxCollider>().size.x / 2f;
        _barrierEff = VFXManager.Instance.TriggerVFX(VFXType.BARRIERSHIELD, transform, rotation: Quaternion.Euler(-90f, 0f, 0f),
            size: new Vector3(x, 1f, 10f), returnAutomatically:false);
    }

    public void InitHealth(float maxHP)
    {
        _barrierStat.maxHealth = maxHP;
        _barrierStat.health = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (_isDestroyed) return;
        
        _barrierStat.health -= damage;
        _damagedEventSo.Raise(this, damage);
        
        ChangeDamageFilter();
        
        if (_barrierStat.health <= 0)
        {
            _barrierStat.health = 0;
            _isDestroyed = true;
            _destroyEventSO.Raise(this);
        }
    }

    private void ChangeDamageFilter()
    {
        float value = _barrierStat.health / _barrierStat.maxHealth;

        switch (value)
        {
            case > 0.66f:
                return;
            case 0:
                _damagedColor = DamagedColor.Gray;
                break;
            case <= .33f:
                _damagedColor = DamagedColor.Red;
                break;
            case <= .66f:
                _damagedColor = DamagedColor.Yellow;
                break;
        }

        var materials = _damagedFilter.materials;
        for (var i = 0; i < materials.Length; i++)
        {
            materials[i] = _damagedMats[(int)_damagedColor];
        }
        _damagedFilter.materials = materials;
    }
}

public enum BarrierType
{
    None,
    Horizontal,
    Vertical, 
    CornerTopLeft,
    CornerTopRight,
    CornerBottomRight, 
    CornerBottomLeft,
    TurnRight,
    TurnLeft,
    Max
}

[Serializable]
public class BarrierStat
{
    public int id;
    public float health;
    public float maxHealth;
}

[Serializable]
public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum DamagedColor
{
    Yellow,
    Red,
    Gray,
}