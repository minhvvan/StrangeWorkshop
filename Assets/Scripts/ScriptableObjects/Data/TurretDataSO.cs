using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretStats", menuName = "SO/Turret/Stats")]
public class TurretDataSO : ScriptableObject
{
    // turret 스탯
    public TurretType turretType;
    public float damage;
    public float attackRange;
    public float fireRate;
    public int maxBulletNum;
    
    // bullet
    public GameObject bullet;
    
    // 모든 turret 공통
    public Color crashedColor;
    
    // 임시
    public float fixSpeed;
    public float lookSpeed;
    public float maxHealth;
}