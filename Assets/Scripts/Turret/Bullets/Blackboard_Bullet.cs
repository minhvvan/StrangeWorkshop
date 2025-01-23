using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    GUN,
    MISSILE,
    MORTAR
}

public class Blackboard_Bullet : MonoBehaviour
{
    [NonSerialized] public TurretType turretType;
    [NonSerialized] public BulletType bulletType;
    
    // bullet status
    [NonSerialized] public float timeToTarget;
    [NonSerialized] public float turnSpeed;
    [NonSerialized] public float knockBack;
    [NonSerialized] public float boomTimer;
    [NonSerialized] public float damageArea;
    
    [NonSerialized] public Transform target;
    [NonSerialized] public float damage;
    
    // 총알의 궤도 계산 방식
    [NonSerialized] public Trajectory trajectory;
    [NonSerialized] public IDamageHandler damageHandler;
    
    public ParticleSystem explosion;
    // Todo: caching하는 방식 말고 자동으로 찾는게 좋지 않을까?
    // Todo: bullet을 objectpooling으로 관리하는 script를 후에 만들어야할듯
    public BulletDataSO bulletDataSO;


    public void Initialize(Transform target, float damage)
    {
        turretType = bulletDataSO.turretType;
        bulletType = bulletDataSO.bulletType;
        timeToTarget = bulletDataSO.timeToTarget;
        turnSpeed = bulletDataSO.turnSpeed;
        knockBack = bulletDataSO.knockBack;
        boomTimer = bulletDataSO.boomTimer;
        damageArea = bulletDataSO.damageArea;
        
        this.target = target;
        this.damage = damage;
        
        
        Bullet bullet = GetComponent<Bullet>();
        switch (bulletType)
        {
            case BulletType.GUN:
            case BulletType.MISSILE:
                trajectory = new Trajectory_Straight(bullet, transform.position, target.position);
                damageHandler = new SingleDamageHandler(bullet);
                break;
            case BulletType.MORTAR: 
                trajectory = new Trajectory_Parabola(bullet, transform.position, target.position); 
                damageHandler = new CircleAreaDamageHandler(bullet);
                break;
        }
    }
}
