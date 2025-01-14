using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFactory
{
    private string _turretPrefabsPath = "Prefabs/Turrets/";
    
    public Turret CreateTurret(TurretType turretType, Transform spawnPoint)
    {
        GameObject turretPrefab = Resources.Load(_turretPrefabsPath + $"{turretType}") as GameObject;

        if (turretPrefab == null)
        {
            Debug.LogError($"Invalid turret type: {turretType}");
        }
        
        GameObject turretInstance = Object.Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);
        Turret turret = turretInstance.GetComponent<Turret>();
        
        switch (turretType)
        {
            case TurretType.BASIC:
                turret.SetShootingStrategy(new SingleShootingStrategy(turret));
                break;
            default:
                break;
        }
        
        return turret;
    }
}
