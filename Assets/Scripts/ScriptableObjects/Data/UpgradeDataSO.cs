using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum TurretUpgradeType
{
    Damage,
    AttackSpeed,
    AttackRange,
    EnergyCost,
    TurretType
}

public enum ModifierMode
{
    Additive,   // 고정 수치 증가 (flat)
    Multiplicative // 배율 증가 (%)
}

[Serializable]
public struct StatModifier
{
    public ModifierMode mode;
    public float value;

    public float ApplyTo(float baseValue, float multiplier)
    {
        return mode switch
        {
            ModifierMode.Additive => baseValue + (value * multiplier),
            ModifierMode.Multiplicative => baseValue * (1f + value),
            _ => baseValue
        };
    }
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "SO/Turret/UpgradeData")]
public class UpgradeDataSO : ScriptableObject
{
    // public float upgradeJackpotProbability;
    public TurretUpgradeType upgradeType;
    public float upgradeTime;
    public int rarity;

    public StatModifier damage;
    public StatModifier attackSpeed;
    public StatModifier attackRange;
    public StatModifier energyCost;
    public TurretType switchingTurretType;
}
