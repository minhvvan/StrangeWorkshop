using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    List<Turret> _turrets = new List<Turret>();

    void CreateTurret(Turret turret)
    {
        Instantiate(turret);
    }

    void CreateTurret(Turret turret, Vector3 position)
    {
        Instantiate(turret, position, Quaternion.identity);
    }

    void DestroyTurret(Turret turret)
    {
        Destroy(turret);
    }

    void ClearTurrets()
    {
        
    }
}
