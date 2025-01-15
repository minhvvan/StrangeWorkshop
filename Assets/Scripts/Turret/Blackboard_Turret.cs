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
    public float fixSpeed = 10;
    public float resizeScale = 2f;
    public Color crashedColor = Color.red;
    public float lookSpeed;
    
    // turret 스탯
    // Todo: scriptable Object로 관리
    public TurretType turretType;
    public float damage;
    public float attackRange;
    public float fireRate;
    public int maxBulletNum;
    public float maxHealth;

    // turret 
    [NonSerialized] public GameObject target;
    [NonSerialized] public int curretBulletNum;
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
    public GameObject bullet;
    public GameObject muzzleEff;

}