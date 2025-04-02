using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class Upgrade : MonoBehaviour
{
    public UpgradeDataSO upgradeData;
    
    private Turret _turret;

    [SerializeField] private ProgressBar _progressBar;

    private float _upgradeProgress;
    
    void Awake()
    {
        _turret = GetComponent<Turret>();
        
        _progressBar.Initialize();
        _progressBar.SetBar(upgradeData.upgradeTime);
        DeactivateUpgradeBar();
    }

    public async UniTask UpgradeProgressively(UpgradeDataSO upgradeData)
    {
        // upgrade 되는데에 시간 소요
        _upgradeProgress = 0f;
        GameObject _upgradeEff = VFXManager.Instance.TriggerVFX(VFXType.TURRETUPGRADE, _turret.gameObject.transform, returnAutomatically: false);
        while (_upgradeProgress < upgradeData.upgradeTime)
        {
            _upgradeProgress += Time.deltaTime;
            _progressBar.UpdateProgressBar(_upgradeProgress);
            await UniTask.Yield();
        }
        _progressBar.ResetBar();
        DeactivateUpgradeBar();
        
        // 업그레이드 완료, 스탯 적용
        await ApplyStatUp(upgradeData);
        VFXManager.Instance.ReturnVFX(VFXType.TURRETUPGRADE, _upgradeEff);
        
        if(upgradeData.upgradeType == TurretUpgradeType.TurretType) Destroy(gameObject);
    }

    private async UniTask ApplyStatUp(UpgradeDataSO upgradeData)
    {
        // 스탯 증가 적용
        _turret.turretData.finalDamage = upgradeData.damage.ApplyTo(_turret.turretData.finalDamage, _turret.turretData.damageMultiplier);
        _turret.turretData.finalAttackSpeed = upgradeData.attackSpeed.ApplyTo(_turret.turretData.finalAttackSpeed, _turret.turretData.attackSpeedMultiplier);
        _turret.turretData.finalAttackRange = upgradeData.attackRange.ApplyTo(_turret.turretData.finalAttackRange, _turret.turretData.attackRangeMultiplier);
        TurretActions.UpdateRangeEffectSize(_turret);
        _turret.turretData.finalEnergyCost = upgradeData.energyCost.ApplyTo(_turret.turretData.finalEnergyCost, _turret.turretData.energyCostMultiplier);

        // 터렛을 스위치하는경우
        if (upgradeData.switchingTurretType != TurretType.None || 
            _turret.turretData.turretType != upgradeData.switchingTurretType)
        {
            await SwitchTurretType(upgradeData.switchingTurretType);
        }
    }

    public void ActivateUpgradeBar()
    {
        _progressBar.gameObject.SetActive(true);
    }

    public void DeactivateUpgradeBar()
    {
        _progressBar.gameObject.SetActive(false);
    }

    public void SetUpgradeBarColor(Color color)
    {
        _progressBar.SetColor(color);
    }

    private async UniTask SwitchTurretType(TurretType newTurretType)
    {
        // 기존 터렛 파괴 후 새로운 object 생성
        GameObject turretPrefab = null;
        switch (newTurretType)
        {
            case TurretType.GUN:
                turretPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Turret.TURRET_GUN);
                break;
            case TurretType.MORTAR:
                turretPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Turret.TURRET_MORTAR);
                break;
            case TurretType.MISSILE:
                turretPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Turret.TURRET_MISSILE);
                break;
            case TurretType.MISSILEDOUBLE:
                turretPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Turret.TURRET_MISSILEDOUBLE);
                break;
            default:
                break;
        }

        if (turretPrefab != null)
        {
            // Turret 위치, parent, 세팅 및 clearcounter의 holdableobject setting
            GameObject newTurretObject = Instantiate(turretPrefab, transform.position, transform.rotation, transform.parent);
            Turret newTurret = newTurretObject.GetComponent<Turret>();
            newTurret.transform.localScale = transform.localScale;
            await UniTask.WaitUntil(() => newTurret.IsInitialized);
            if (newTurret.turretData.parentClearCounter != null)
            {
                newTurret.turretData.parentClearCounter.ClearHoldableObject();
                newTurret.turretData.parentClearCounter.SetHoldableObject(newTurret);
            }

            // 터렛이 가진 스탯 배율에 따라 스탯 재조정
            newTurret.turretData.finalDamage = _turret.turretData.finalDamage / _turret.turretData.damageMultiplier * newTurret.turretData.damageMultiplier;
            newTurret.turretData.finalAttackRange = _turret.turretData.finalAttackRange / _turret.turretData.attackRangeMultiplier * newTurret.turretData.attackRangeMultiplier;
            newTurret.turretData.finalAttackSpeed = _turret.turretData.finalAttackSpeed / _turret.turretData.attackSpeedMultiplier * newTurret.turretData.attackSpeedMultiplier;
            newTurret.turretData.finalEnergyCost = _turret.turretData.finalEnergyCost / _turret.turretData.energyCostMultiplier * newTurret.turretData.energyCostMultiplier;
        }
    }
}