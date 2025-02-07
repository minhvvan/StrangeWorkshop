using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class UpgradeStats
{
    public float damage;
    public float fireRate;
    public float attackRange;
    public int maxBulletNum;
}

public class Upgrade : MonoBehaviour
{
    public UpgradeDataSO upgradeData;
    
    private int _currentUpgradeLevel;
    private Turret _turret;
    // level이 올라갈 때 증가하는 수치들을 담은 data
    private List<UpgradeStats> _upgrades;
    [SerializeField] private ProgressBar _progressBar;

    private float _upgradeProgress;

    void Awake()
    {
        _turret = GetComponent<Turret>();
        _currentUpgradeLevel = 0;
        _upgrades = upgradeData.upgrades;
        _progressBar.Initialize();
        _progressBar.SetBar(upgradeData.upgradeTime);
        DeactivateUpgradeBar();
    }
    
    public bool Upgradable()
    {
        // 업그레이드 가능한지 판별
        if (_currentUpgradeLevel >= _upgrades.Count || _turret.turretData.isUpgrading) return false;
        
        return true;
    }

    public bool UpgradeProgressively()
    {
        _upgradeProgress += Time.deltaTime;
        _progressBar.UpdateProgressBar(_upgradeProgress);
        if (_upgradeProgress >= upgradeData.upgradeTime)
        {
            _upgradeProgress = 0f;
            _progressBar.ResetBar();
            DeactivateUpgradeBar();
            
            UpgradeLevelRandomly();
            return true;
        }
        return false;
    }

    public void UpgradeLevelOne()
    {
        // turret의 level 개념이 존재하고 level 증가에 따라 스탯 증가하는 방식
        // level을 1씩 올린다.
        // Todo: counter에서 upgrade 가능여부를 처리하면 조건문 삭제
        if (_currentUpgradeLevel < _upgrades.Count)
        {
            ApplyStatUp();
            UpgradeEffect(1);
        }
    }

    public void UpgradeLevelRandomly()
    {
        // turret의 level을 랜덤하게 올린다
        // 일단 1레벨 올려주고 더 올라갈 때마다 1/n 확률로 성공
        int increasedLevel = 0;
        // Todo: counter에서 upgrade 가능여부를 처리하면 조건문 삭제
        if (_currentUpgradeLevel < _upgrades.Count)
        {
            ApplyStatUp();
            increasedLevel++;
            UpgradeLevelRandomlyHelper(ref increasedLevel);
        }
        UpgradeEffect(increasedLevel);
    }

    private void UpgradeLevelRandomlyHelper(ref int increasedLevel)
    {
        // Todo: counter에서 upgrade 가능여부를 처리하면 조건문 삭제
        if (_currentUpgradeLevel < _upgrades.Count && Random.value <=  upgradeData.upgradeJackpotProbability)
        {
            ApplyStatUp();
            increasedLevel++;
            UpgradeLevelRandomlyHelper(ref increasedLevel);
        }
    }

    private void ApplyStatUp()
    {
        // 스탯 증가 적용
        // currentupgradelevel 증가
        _turret.turretData.damage += _upgrades[_currentUpgradeLevel].damage;
        _turret.turretData.fireRate -= _upgrades[_currentUpgradeLevel].fireRate;
        _turret.turretData.attackRange += _upgrades[_currentUpgradeLevel].attackRange;
        _turret.turretActions.UpdateRangeEffectSize();
        _turret.turretData.maxBulletNum += _upgrades[_currentUpgradeLevel].maxBulletNum;
        
        _currentUpgradeLevel++;
    }

    private void UpgradeEffect(int increasedLevel)
    {
        // 한번에 증가한 level에 따라 effect 적용
        // Todo: 레벨에 따른 이펙트 찾기
        switch (increasedLevel)
        {
            case 1:
                VFXManager.Instance.TriggerVFX(VFXType.UPGRADE1, _turret.transform);
                break;
            case 2:
                VFXManager.Instance.TriggerVFX(VFXType.UPGRADE2, _turret.transform);
                break;
            default:
                VFXManager.Instance.TriggerVFX(VFXType.UPGRADE3, _turret.transform);
                break;
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
}