using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    BASIC
}

public class Blackboard_Turret : MonoBehaviour
{
    // turret head position
    public Transform turretHead;
    public float lookSpeed;
    
    // turret status
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
    
    // 포탑 종류, 전략패턴
    public TurretType turretType = TurretType.BASIC;
    public ITargetStrategy targetStrategy;
    public ShootingStrategy shootingStrategy;
}