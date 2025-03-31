using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretBaseStats", menuName = "SO/Turret/BaseStats")]
public class TurretBaseStatSO : ScriptableObject
{
    // turret 기본 스탯
    public float damage;
    public float attackRange;
    public float fireRate;
    public float energyCost;
}