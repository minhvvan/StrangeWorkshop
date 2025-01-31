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
        _timer = _controller.turretData.fireRate;
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_controller.turretData.target != null && _controller.turretData.currentBulletNum > 0)
        {
            _controller.turretData.shootingStrategy.FollowTarget(_controller.turretData.target);
            if (_controller.turretData.fireRate <= _timer)
            {
                _controller.turretData.shootingStrategy.Shoot(_controller.turretData.target);
                _timer = 0f;
                ChangeState();
            }
        }
        Debug.Log("attack");
        ChangeState();
    }

    public override void Exit()
    {
        // timer 초기화
        _timer = _controller.turretData.fireRate;
    }
    
    private void ChangeState()
    {
        // 작동 가능한지 체크 -> target이 있는지 체크 
        if (_turretData.isCrashed)
        {
            _controller.SetState(_controller.crashState);
        }
        else if (!_turretData.isOnCounter || _turretData.currentBulletNum <= 0 || _turretData.isUpgrading)
        {
            _controller.SetState(_controller.notWorkingState);
        }
        else if (_turretData.target == null)
        {
            _controller.SetState(_controller.idleState);
        }
    }
}