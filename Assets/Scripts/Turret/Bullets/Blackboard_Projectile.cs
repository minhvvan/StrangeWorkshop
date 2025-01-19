using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    GUN,
    MISSILE,
    MORTAR
}

public class Blackboard_Projectile : MonoBehaviour
{
    public TurretType type = TurretType.MORTAR;
    public BulletType bulletType;
    public Transform target;

    // bullet status
    public float timeToTarget = 1f;
    public float turnSpeed = 1f;
    public float knockBack = 0.1f;

    public float damage;
    // 터질때까지의 최대 시간
    public float boomTimer = 5f;
    
    public ParticleSystem explosion;
    
    // 총알의 궤도 계산 방식
    public Trajectory trajectory;
}
