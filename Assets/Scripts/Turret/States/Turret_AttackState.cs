using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        _turretData.attackRateCancelToken.Dispose();
        _turretData.attackRateCancelToken = new CancellationTokenSource();
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_turretData.target != null &&
            _turretData.parentClearCounter != null &&
            !_turretData.parentClearCounter.OutOfEnergy(_turretData.finalEnergyCost))
        {
            _turretData.shootingStrategy.FollowTarget(_turretData.target);
            if (1 / _turretData.finalAttackSpeed <= _timer)
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
        SetTimer();
    }
    
    private void ChangeState()
    {
        // 작동 가능한지 체크 -> target이 있는지 체크 
        if (_turretData.parentClearCounter == null || _turretData.parentClearCounter.OutOfEnergy(_turretData.finalEnergyCost) || _turretData.isUpgrading)
        {
            _controller.SetState(_controller.notWorkingState);
        }
        else if (_turretData.target == null)
        {
            _controller.SetState(_controller.idleState);
        }
    }

    private async UniTask SetTimer()
    {
        float startTime = Time.time;
        // attackState를 빠져나가도 timer가 일정시간동안은 유지되도록 설정
        while (_timer < 1 / _turretData.finalAttackSpeed)
        {
            _timer += Time.time - startTime;
            await UniTask.Yield(cancellationToken: _turretData.attackRateCancelToken.Token);
        }

    }
}