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
        // 중간에 crash 되도 업그레이드 진척도는 저장된다
        if (_turretData.isUpgrading)
        {
            if (_controller.turretUpgrade.UpgradeProgressively())
            {
                // upgrade가 완료될 시 진입
                _turretData.isUpgrading = false;
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
