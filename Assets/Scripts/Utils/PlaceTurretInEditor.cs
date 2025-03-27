using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaceTurretOnClearCounter))]
public class PlaceTurretInEditor : Editor
{
     PlaceTurretOnClearCounter turret;
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            turret = (PlaceTurretOnClearCounter)target;
            turret.UpdateClosestCounter();
        }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            turret.Disabled();
        }
    }
}
