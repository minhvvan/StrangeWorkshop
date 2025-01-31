using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Turret_NotWorkingState : BaseState<Turret>
{
    /*
     * 공격을 할 수 없는 상태를 의미한다.
     * 아래와 같은 상황에서 진입한다
     * 플레이어가 들고 있다, 총알이 없다, 업그레이드 중이다. 
     */
    
    private Blackboard_Turret _turretData;
    
    private float _elapsedUpgradeTime;
    // Start is called before the first frame update
    public Turret_NotWorkingState(Turret controller) : base(controller)
    {
        _turretData = _controller.turretData;
    }
    
    public override void Enter()
    {
    }

    public override void UpdateState()
    {
        // 총알이 있는가?
        if (_turretData.currentBulletNum > 0) _turretData.noAmmoImage.SetActive(false);
        else _turretData.noAmmoImage.SetActive(true);
        
        // 업그레이드중인가?
        if (_turretData.isUpgrading)
        {
            _elapsedUpgradeTime += Time.deltaTime;
            // Todo: upgrade 진척도 slidebar로 표시
            
            if (_elapsedUpgradeTime >= _controller.turretUpgrade.upgradeData.upgradeTime)
            {
                _turretData.isUpgrading = false;
                _elapsedUpgradeTime = 0f;
                _controller.turretUpgrade.UpgradeLevelRandomly();
            }
        }
        ChangeState();
    }

    public override void Exit()
    {
    }
    
    private void ChangeState()
    {
        // 고장났는지 체크 -> 작동 가능한지 체크 -> target이 있는지 체크
        if (_turretData.isCrashed)
        {
            _controller.SetState(_controller.crashState);
        }
        else if (_turretData.isOnCounter && _turretData.currentBulletNum > 0 && !_turretData.isUpgrading)
        {
            if(_turretData.target != null) { _controller.SetState(_controller.attackState); }
            else { _controller.SetState(_controller.idleState); }
        }
    }
}
