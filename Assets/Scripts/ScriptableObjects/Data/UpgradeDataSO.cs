using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "SO/Turret/UpgradeData")]
public class UpgradeDataSO : ScriptableObject
{
    public float upgradeJackpotProbability;
    
    public List<UpgradeStats> upgrades = new List<UpgradeStats>();

    private void OnEnable()
    {
        if(upgradeJackpotProbability == 0f) upgradeJackpotProbability = 0.125f;
    }
}
