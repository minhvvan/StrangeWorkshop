using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public List<TurretDataSO> turretDataSOs;

    private List<Turret> _turrets;
    private TurretFactory _turretFactory;

    public Transform a;
    public Transform b;
    
    void Awake()
    {
        _turrets = new List<Turret>();
        _turretFactory = new TurretFactory(turretDataSOs);
        
        // testing
        CreateTurret(TurretType.BASIC, a);
        CreateTurret(TurretType.MISSILE, b);
        CreateTurret(TurretType.MORTAR);
    }
    
    void CreateTurret(TurretType turretType)
    {
        _turrets.Add(_turretFactory.CreateTurret(turretType, transform));
    }

    void CreateTurret(TurretType turretType, Transform parentTransform)
    {
        _turrets.Add(_turretFactory.CreateTurret(turretType, parentTransform));
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
