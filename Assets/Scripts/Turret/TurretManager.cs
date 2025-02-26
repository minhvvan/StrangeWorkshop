using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : Singleton<TurretManager>
{
    private List<Turret> _turrets;
    public bool rangeEffOn { get; set; }

    void Awake()
    {
        _turrets = new List<Turret>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleRangeEff();
        }
    }

    public void AddTurret(Turret turret)
    {
        if(!_turrets.Contains(turret))
            _turrets.Add(turret);
    }

    public void RemoveTurret(Turret turret)
    {
        if(_turrets.Contains(turret))
            _turrets.Remove(turret);
    }

    private void ToggleRangeEff()
    {
        rangeEffOn = !rangeEffOn;
        foreach (Turret turret in _turrets)
        {
            turret.turretData.rangeEff.SetActive(rangeEffOn);
        }
    }

    // void DestroyTurret(Turret turret)
    // {
    //     _turrets.Remove(turret);
    //     Destroy(turret.gameObject);
    // }
    //
    void ClearTurrets()
    {
        foreach (Turret turret in _turrets)
        {
            Destroy(turret.gameObject);
        }
        _turrets.Clear();
    }
}
