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
    public int energyCost;
    
    // bullet
    public GameObject bullet;
    
    // 임시
    public float lookSpeed;
}