using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretBaseStats", menuName = "SO/Turret/BaseStats")]
public class TurretBaseStatSO : ScriptableObject
{
    // 모든 터렛이 가지는 turret 기본 스탯
    public float damage;
    public float attackRange;
    public float attackSpeed;
    public float energyCost;
}