using UnityEngine;
using DG.Tweening;

public class ProcessCounter_OverState : BaseState<ProcessCounter>
{
    public ProcessCounter_OverState(ProcessCounter controller) : base(controller) {}

    private float _currentTime;
    private bool isOver = false;

    private GameObject _blackSmokingEff;
    public override void Enter()
    {
        //UI, Warning - SetActive true
        _controller.progressBar.SetBar(_controller.burnTime);
        _controller.progressBar.SetColor(Color.yellow);
        
        _blackSmokingEff = VFXManager.Instance.TriggerVFX(VFXType.PROCESSCOUNTEROVERWORKING, _controller.transform,
            rotation: Quaternion.Euler(-90f, 0, 0), returnAutomatically: false);
    }

    public override void UpdateState()
    {
        _currentTime += Time.deltaTime;
        _controller.progressBar.UpdateProgressBar(_currentTime);
        if (_currentTime >= _controller.burnTime/2 && !isOver)
        {
            _controller.progressBar.SetColorLerp(Color.red);
            _controller.progressBar.gameObject.transform.DOShakePosition(_controller.burnTime/2, new Vector3(0.3f,0,0.1f) , 15, 1, false, false);
            isOver = true;
        }
        if (_currentTime >= _controller.burnTime)
        {
            _controller.ClearHoldableObject();
            HoldableObject.SpawnHoldableObject(_controller.burnSO, _controller, _controller.GetHoldableObjectFollowTransform());
            _controller.SetState(_controller._noneState);
        }
    }

    public override void Exit()
    {
        _currentTime = 0f;
        isOver = false;
        _controller.progressBar.ResetBar();
        VFXManager.Instance.ReturnVFX(VFXType.PROCESSCOUNTEROVERWORKING, _blackSmokingEff);
    }
}