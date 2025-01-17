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
    // turret head position
    public Transform turretHead;
    public float lookSpeed;
    
    // turret status
    // Todo: scriptable Object로 관리
    public TurretType turretType;
    public float damage;
    public float attackRange;
    public float fireRate;
    public int maxBulletNum;

    // Prefabs
    [Header("Prefabs")] 
    public GameObject rangeEff;
    public Transform muzzleMain;
    public GameObject bullet;
    public GameObject muzzleEff;
    
    // 전략패턴
    public ITargetStrategy targetStrategy;
    public ShootingStrategy shootingStrategy;
}