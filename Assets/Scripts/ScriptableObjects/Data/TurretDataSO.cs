using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretStats", menuName = "SO/Turret/Stats")]
public class TurretDataSO : ScriptableObject
{
    public TurretBaseStatSO turretBaseStatSO;
    public TurretType turretType;
    
    // bullet
    public GameObject bullet;
    
    // 스탯 배율
    public float damageMultiplier;
    public float attackRangeMultiplier;
    public float fireRateMultiplier;
    public float energyCostMultiplier;
    
    // 임시
    public float lookSpeed;
}