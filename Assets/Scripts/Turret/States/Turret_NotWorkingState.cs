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
     * 플레이어가 들고 있다, 에너지가 없다, 업그레이드 중이다. 
     */
    private Blackboard_Turret _turretData;
    
    private Color[] _previousColors;
    // Start is called before the first frame update
    public Turret_NotWorkingState(Turret controller) : base(controller)
    {
        _turretData = _controller.turretData;
        _previousColors = new Color[_turretData.renderers.Length];
    }
    
    public override void Enter()
    {
        for (int i = 0; i < _turretData.renderers.Length; i++)
        {
            _previousColors[i] = _turretData.renderers[i].material.color;
        }
    }

    public override void UpdateState()
    {
        if (_turretData.parentClearCounter == null || _turretData.parentClearCounter.OutOfEnergy(_turretData.finalEnergyCost))
        {
            for (int i = 0; i < _turretData.renderers.Length; i++)
            {
                _turretData.renderers[i].material.color = _turretData.deactivatedColor;
            }
        }
        else
        {
            for (int i = 0; i < _turretData.renderers.Length; i++)
            {
                _turretData.renderers[i].material.color = _previousColors[i];
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
        if (_turretData.parentClearCounter != null && !_turretData.isUpgrading && !_turretData.parentClearCounter.OutOfEnergy(_turretData.finalEnergyCost))
        {
            if(_turretData.target != null) { _controller.SetState(_controller.attackState); }
            else { _controller.SetState(_controller.idleState); }
        }
    }
}
