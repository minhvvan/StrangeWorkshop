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
        Debug.Log("Enter AttackState");
        // timer 초기화
        _timer = _controller.turret.fireRate;
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_controller.target != null && _controller.remainingBulletsNum > 0)
        {
            _controller.turret.shootingStrategy.FollowTarget(_controller.target);
            // FollowTarget(_controller.target);
            if (_controller.turret.fireRate <= _timer)
            {
                _controller.turret.shootingStrategy.Shoot(_controller.target);
                // Shoot(_controller.target);
                _timer = 0f;
            }
        }
        ChangeState();
    }

    public override void Exit()
    {
        Debug.Log("Exit AttackState");
        // timer 초기화
        _timer = _controller.turret.fireRate;
    }
    
    private void ChangeState()
    {
        /*
         State 변경 조건 확인
         state 변경 여부 체크 순서:
         turret이 counter에 있는가? -> turret이 고장났는가? -> turret이 총알이 없는가? -> 적이 있는가?
         */
        if (!_controller.isOnCounter)
        {
            Debug.Log("Change to HoldState");
            _controller.SetState(_controller.holdState);
        }
        else if (_controller.isCrashed)
        {
            Debug.Log("Change to CrashState");
            _controller.SetState(_controller.crashState);
        }
        else if (_controller.remainingBulletsNum <= 0)
        {
            Debug.Log("Change to EmptyState");
            _controller.SetState(_controller.emptyState);
        }
        else if (_controller.target is null)
        {
            Debug.Log("Change to IdleState");
            _controller.SetState(_controller.idleState);
        }
    }
    
    public void Shoot(GameObject target)
    {
        // target 향해서 총알 발사
        if (_controller.turret.turretType == TurretType.SINGLE)
        {
            Object.Instantiate(_controller.turret.muzzleEff, _controller.turret.muzzleMain);
            GameObject missleGo = Object.Instantiate(_controller.turret.bullet, _controller.turret.muzzleMain);
            Projectile projectile = missleGo.GetComponent<Projectile>();
            projectile.target = target.transform;
        }
        _controller.remainingBulletsNum--;
    }

    public void FollowTarget(GameObject target)
    {
        // target 방향으로 터렛 헤드 돌리기
        Transform turretHead = _controller.turret.turretHead;
        Vector3 targetDir = target.transform.position - turretHead.position;
        targetDir.y = 0;
        
        //turreyHead.forward = targetDir;
        if (_controller.turret.turretType == TurretType.SINGLE)
        {
            turretHead.forward = targetDir;
        }
        else
        {
           turretHead.transform.rotation = Quaternion.RotateTowards(turretHead.rotation, Quaternion.LookRotation(targetDir), _controller.turret.lookSpeed * Time.deltaTime);
        }
    }
}