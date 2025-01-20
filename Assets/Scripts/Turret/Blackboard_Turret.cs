using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    BASIC,
    MISSILE,
    MORTAR
}

public class Blackboard_Turret : MonoBehaviour
{
    // 모든 turret 공통
    [NonSerialized] public float fixSpeed = 10;
    [NonSerialized] public float resizeScale = 2f;
    [NonSerialized] public Color crashedColor = Color.red;
    [NonSerialized] public float lookSpeed;
    
    // turret 스탯
    [NonSerialized] public TurretType turretType;
    [NonSerialized] public float damage;
    [NonSerialized] public float attackRange;
    [NonSerialized] public float fireRate;
    [NonSerialized] public int maxBulletNum;
    [NonSerialized] public float maxHealth;
    
    // bullet
    [NonSerialized] public GameObject bullet;

    // turret 
    [NonSerialized] public GameObject target;
    [NonSerialized] public int currentBulletNum;
    [NonSerialized] public bool isOnCounter = true;
    [NonSerialized] public bool isCrashed = false;
    [NonSerialized] public float currentHealth;
    
    // 전략패턴
    public ITargetStrategy targetStrategy;
    public ShootingStrategy shootingStrategy;
    
    // player (손)위치
    public Transform playerHandTransform;
    
    // caching
    [Header("caching")]
    public Transform turretHead;
    public GameObject rangeEff;
    public MeshRenderer[] renderers;
    public GameObject noAmmoImage;
    
    public Transform muzzleMain;
    public GameObject muzzleEff;

    public void InitData(TurretDataSO so)
    {
        // stat 초기화
        fixSpeed = so.fixSpeed;
        resizeScale = so.resizeScale;
        crashedColor = so.crashedColor;
        lookSpeed = so.lookSpeed;
        turretType = so.turretType;
        damage = so.damage;
        attackRange = so.attackRange;
        fireRate = so.fireRate;
        maxBulletNum = so.maxBulletNum;
        maxHealth = so.maxHealth;
        bullet = so.bullet;
        
        // status
        currentBulletNum = maxBulletNum;
        currentHealth = maxHealth;
        targetStrategy = new ClosestTargetStrategy();
        float size = attackRange * 2f;
        rangeEff.transform.localScale = new Vector3(size, size, 1f);
    }
}