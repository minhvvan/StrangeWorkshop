using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private float maxEnergy = 100f;
    private float currentEnergy;
    [SerializeField] private float chargeInterval = 1f;
    [SerializeField] private float chargeSpeed = 10f;
    
    private bool _hasTurret = false;
    private CancellationTokenSource _cancelChargeTokenSource;

    private void Awake()
    {
        currentEnergy = maxEnergy;
        _cancelChargeTokenSource = new CancellationTokenSource();

        Turret existedTurret = GetComponentInChildren<Turret>();
        if (existedTurret != null)
        {
            SetHoldableObject(existedTurret);
        }
    }
    
    public override void Interact(IHoldableObjectParent parent)
    {
        if (!HasHoldableObject()) // clearcounter에 아무것도 없음
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
            
            if (!parent.HasHoldableObject()) // 들기
            {
                GiveHoldableObject(parent);
                TakeOffPlayerGlove(parent);
                startCharging();
            }
        }
    }

    public async UniTask startCharging()
    {
        _cancelChargeTokenSource = new CancellationTokenSource();
        while (currentEnergy < maxEnergy)
        {
            await UniTask.WaitForSeconds(chargeInterval, cancellationToken:_cancelChargeTokenSource.Token);
            currentEnergy += chargeSpeed;
            Debug.Log(currentEnergy);
        }

        if (!_cancelChargeTokenSource.IsCancellationRequested)
        {
            currentEnergy = maxEnergy;
        }
    }

    public void UseEnergy(float energy)
    {
        currentEnergy -= energy;
        Debug.Log(currentEnergy);
    }

    public void ChangeMaxEnergy(float dMaxEnergy)
    {
        maxEnergy += dMaxEnergy;
    }

    public void ChangeChargeInterval(float dChargeInterval)
    {
        chargeInterval += dChargeInterval;
    }

    public void ChangeChargeSpeed(float dChargeSpeed)
    {
        chargeSpeed += dChargeSpeed;
    }

    public bool OutOfEnergy(float cost)
    {
        return currentEnergy < cost;
    }
}
