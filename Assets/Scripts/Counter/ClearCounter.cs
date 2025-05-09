using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ClearCounter : BaseCounter
{
    [SerializeField] private Barrier _outerBarrier;
    [SerializeField] private float _maxEnergy = 100f;
    private float _currentEnergy;
    [SerializeField] private float _chargeInterval = 1f;
    [SerializeField] private float _chargeSpeed = 10f;
    [SerializeField] private BarrierCounterUI _barrierCounterUI;

    private CancellationTokenSource _cancelChargeTokenSource;

    private void Awake()
    {
        _currentEnergy = _maxEnergy;
        _cancelChargeTokenSource = new CancellationTokenSource();

        Turret existedTurret = GetComponentInChildren<Turret>();
        if (existedTurret != null)
        {
            SetHoldableObject(existedTurret);
        }
    }

    private void Start()
    {
        _barrierCounterUI.InitMaxEnergy(_maxEnergy);
        _barrierCounterUI.SetEnergy(_currentEnergy);
    }

    public override void Interact(IInteractAgent agent = null)
    {
        if(_outerBarrier.Destroyed) return;
        
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (!HasHoldableObject())
            {
                if (parent.HasHoldableObject())
                {
                    parent.GiveHoldableObject(this);
                    _cancelChargeTokenSource.Cancel();
                    _cancelChargeTokenSource.Dispose();
                }
            }
            else
            {
                if (parent.HasHoldableObject())
                {
                    if (GetHoldableObject().Acceptable(parent.GetHoldableObject()))
                    {
                        parent.ClearHoldableObject();
                    }
                }
                
                else if (!parent.HasHoldableObject())
                {
                    GiveHoldableObject(parent);
                    TakeOffPlayerGlove(parent);
                    UniTask.Void(async () => await StartCharging());
                }
            }
        }
    }

    private async UniTask StartCharging()
    {
        // counter 위에 아무것
        _cancelChargeTokenSource = new CancellationTokenSource();
        while (_currentEnergy < _maxEnergy)
        {
            await UniTask.WaitForSeconds(_chargeInterval, cancellationToken:_cancelChargeTokenSource.Token);
            _currentEnergy += _chargeSpeed;
            _barrierCounterUI.SetEnergy(_currentEnergy);
        }

        if (!_cancelChargeTokenSource.IsCancellationRequested)
        {
            _currentEnergy = _maxEnergy;
            _barrierCounterUI.SetEnergy(_currentEnergy);
        }
    }

    public void UseEnergy(float energy)
    {
        _currentEnergy -= energy;
        _barrierCounterUI.SetEnergy(_currentEnergy);
    }

    public void ChangeMaxEnergy(float dMaxEnergy)
    {
        _maxEnergy += dMaxEnergy;
    }

    public void ChangeChargeInterval(float dChargeInterval)
    {
        _chargeInterval += dChargeInterval;
    }

    public void ChangeChargeSpeed(float dChargeSpeed)
    {
        _chargeSpeed += dChargeSpeed;
    }

    public bool OutOfEnergy(float cost)
    {
        return _currentEnergy < cost;
    }

    public bool RepairCounter(float value)
    {
        return _outerBarrier.Repair(value);
    }
}
