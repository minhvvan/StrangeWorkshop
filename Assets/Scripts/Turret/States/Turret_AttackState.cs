using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret_AttackState : BaseState<Turret>
{
    public Turret_AttackState(Turret controller) : base(controller){ }

    private float _timer;

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
            // FollowTarget(_controller.target);
            if (_controller.turretData.fireRate <= _timer)
            {
                _controller.turretData.shootingStrategy.Shoot(_controller.turretData.target);
                // Shoot(_controller.target);
                _timer = 0f;
            }
        }
        ChangeState();
    }

    public override void Exit()
    {
        // timer 초기화
        _timer = _controller.turretData.fireRate;
    }
    
    private void ChangeState()
    {
        /*
         State 변경 조건 확인
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         */
        if (!_controller.turretData.isOnCounter)
        {
            _controller.SetState(_controller.holdState);
        }
        else if (_controller.turretData.isCrashed)
        {
            _controller.SetState(_controller.crashState);
        }
        else if (_controller.turretData.currentBulletNum <= 0)
        {
            _controller.SetState(_controller.emptyState);
        }
        else if (_controller.turretData.target is null)
        {
            _controller.SetState(_controller.idleState);
        }
    }
}