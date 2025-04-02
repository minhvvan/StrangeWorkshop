using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurretManager : Singleton<TurretManager>
{
    private List<Turret> _turrets;
    public bool rangeEffOn { get; set; }
    public bool IsInitialized { get; private set; }

    void Start()
    {
        _turrets = new List<Turret>();
        IsInitialized = true;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleRangeEff();
        }
    }

    public async void AddTurret(Turret turret)
    {
        await UniTask.WaitUntil(() => IsInitialized && QuestManager.Instance.IsInitialized);
        if(!_turrets.Contains(turret))
            _turrets.Add(turret);
        QuestManager.Instance.Notify(QuestType.LimitedTurret, 1);
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
    
    void ClearTurrets()
    {
        foreach (Turret turret in _turrets)
        {
            Destroy(turret.gameObject);
        }
        _turrets.Clear();
    }
}
