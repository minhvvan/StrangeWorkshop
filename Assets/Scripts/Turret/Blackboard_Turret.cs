using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    SINGLE
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

    // shooting prefabs
    [Header("Shooting")]
    public TurretType turretType = TurretType.SINGLE;
    public Transform muzzleMain;
    public GameObject bullet;
    public GameObject muzzleEff;
    
    // 전략패턴
    public ITargetStrategy targetStrategy;
    public ShootingStrategy shootingStrategy;
}