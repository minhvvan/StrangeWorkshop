using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret_AttackState : BaseState<Turret>
{
    private Blackboard_Turret _turretData;
    private float _timer;

    public Turret_AttackState(Turret controller) : base(controller)
    {
        _turretData = _controller.turretData;
    }

    public override void Enter()
    {
        // timer 초기화
        _timer = _turretData.fireRate;
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        // if (_controller.turretData.target != null && _controller.turretData.currentBulletNum > 0)
        if (_turretData.target != null && !_turretData.parentClearCounter.OutOfEnergy(_turretData.energyCost))
        {
            _turretData.shootingStrategy.FollowTarget(_turretData.target);
            if (_turretData.fireRate <= _timer)
            {
                _turretData.shootingStrategy.Shoot(_turretData.target);
                _timer = 0f;
                ChangeState();
            }
        }
        ChangeState();
    }

    public override void Exit()
    {
        // timer 초기화
        _timer = _turretData.fireRate;
    }
    
    private void ChangeState()
    {
        // 작동 가능한지 체크 -> target이 있는지 체크 
        // if (_turretData.isCrashed)
        // {
        //     _controller.SetState(_controller.crashState);
        // }
        if (_turretData.parentClearCounter == null || _turretData.parentClearCounter.OutOfEnergy(_turretData.energyCost) || _turretData.isUpgrading)
        {
            _controller.SetState(_controller.notWorkingState);
        }
        else if (_turretData.target == null)
        {
            _controller.SetState(_controller.idleState);
        }
    }
}