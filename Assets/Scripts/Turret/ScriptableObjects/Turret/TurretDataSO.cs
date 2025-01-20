using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretStats", menuName = "SO/Turret/Stats")]
public class TurretDataSO : ScriptableObject
{
    // 모든 turret 공통
    public float fixSpeed;
    public float resizeScale;
    public Color crashedColor;
    public float lookSpeed;
    
    // turret 스탯
    public TurretType turretType;
    public float damage;
    public float attackRange;
    public float fireRate;
    public int maxBulletNum;
    public float maxHealth;
    
    // bullet
    public GameObject bullet;
}