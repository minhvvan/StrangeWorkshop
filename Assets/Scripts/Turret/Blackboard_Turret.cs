using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    None,
    GUN,
    MISSILE,
    MORTAR,
    MISSILEDOUBLE,
    Max
}

public class Blackboard_Turret : MonoBehaviour
{
    // so
    public TurretDataSO so;
    
    // 모든 turret 공통
    [NonSerialized] public Color deactivatedColor = Color.gray;
    [NonSerialized] public float lookSpeed;
    
    // turret 스탯
    [NonSerialized] public TurretType turretType;
    [NonSerialized] public float finalDamage;
    [NonSerialized] public float finalAttackRange;
    [NonSerialized] public float finalFireRate;
    [NonSerialized] public float finalEnergyCost;
    
    // turret 스탯 배율
    [NonSerialized] public float damageMultiplier;
    [NonSerialized] public float attackRangeMultiplier;
    [NonSerialized] public float fireRateMultiplier;
    [NonSerialized] public float energyCostMultiplier;
    
    // bullet
    [NonSerialized] public GameObject bullet;

    // turret 상태정보
    [NonSerialized] public GameObject target;
    [NonSerialized] public bool isUpgrading = false;

    // 전략패턴
    public ITargetStrategy targetStrategy;
    public ShootingStrategy shootingStrategy;
    
    // parent
    [NonSerialized] public ClearCounter parentClearCounter;
    
    // caching
    [Header("caching")]
    public Transform turretHead;
    public GameObject rangeEff;
    public MeshRenderer[] renderers;
    
    public Transform muzzleMain;
    public Transform muzzleSub;
    
    public ProgressBar progressBarFix;

    public void Initialize()
    {
        // stat 초기화
        lookSpeed = so.lookSpeed;
        turretType = so.turretType;
        finalDamage = so.turretBaseStatSO.damage * so.damageMultiplier;
        finalAttackRange = so.turretBaseStatSO.attackRange * so.attackRangeMultiplier;
        finalFireRate = so.turretBaseStatSO.fireRate * so.fireRateMultiplier;
        finalEnergyCost = so.turretBaseStatSO.energyCost * so.energyCostMultiplier;
        bullet = so.bullet;
        
        // stat 배율 초기화
        damageMultiplier = so.damageMultiplier;
        attackRangeMultiplier = so.attackRangeMultiplier;
        fireRateMultiplier = so.fireRateMultiplier;
        energyCostMultiplier = so.energyCostMultiplier;
        
        // range effect
        float size = finalAttackRange * 2f;
        rangeEff.transform.localScale = new Vector3(size, size, 1f);
        rangeEff.SetActive(TurretManager.Instance.rangeEffOn);
        
        // BarUI
        progressBarFix.Initialize();
        progressBarFix.gameObject.SetActive(false);
        
        // init strategies
        targetStrategy = new ClosestTargetStrategy();
        switch (turretType)
        {
            case TurretType.GUN:
            case TurretType.MISSILE:
            case TurretType.MORTAR:
                shootingStrategy = new SingleShootingStrategy(GetComponent<Turret>());
                break;
            case TurretType.MISSILEDOUBLE:
                shootingStrategy = new DoubleShootingStrategy(GetComponent<Turret>());
                break;
            default:
                break;
        }
        
        // parent clear counter 초기화
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
            {
                parentClearCounter = clearCounter;
                break;
            }
            current = current.parent;
        }
    }
}