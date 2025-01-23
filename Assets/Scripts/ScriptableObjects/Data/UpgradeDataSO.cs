using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "SO/Turret/UpgradeData")]
public class UpgradeDataSO : ScriptableObject
{
    public List<UpgradeLevel> upgrades = new List<UpgradeLevel>();
}
