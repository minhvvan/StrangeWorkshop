using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    List<Turret> _turrets = new List<Turret>();
    TurretFactory _turretFactory = new TurretFactory();

    void Awake()
    {
        CreateTurret(TurretType.BASIC);
    }
    
    void CreateTurret(TurretType turretType)
    {
        _turrets.Add(_turretFactory.CreateTurret(turretType, transform));
    }

    void DestroyTurret(Turret turret)
    {
        _turrets.Remove(turret);
        Destroy(turret.gameObject);
    }

    void ClearTurrets()
    {
        foreach (Turret turret in _turrets)
        {
            Destroy(turret.gameObject);
        }
        _turrets.Clear();
    }
}
