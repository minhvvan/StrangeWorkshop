using UnityEngine;

public class ProcessCounter_OverState : BaseState<ProcessCounter>
{
    public ProcessCounter_OverState(ProcessCounter controller) : base(controller) {}

    private float _currentTime;
    public override void Enter()
    {
        //UI, Warning - SetActive true
        _controller.progressBar.SetBar(_controller.burnTime);
        _controller.progressBar.SetColor(Color.yellow);
    }

    public override void UpdateState()
    {
        _currentTime += Time.deltaTime;
        _controller.progressBar.UpdateProgressBar(_currentTime);
        if (_currentTime >= _controller.burnTime)
        {
            _controller.ClearHoldableObject();
            HoldableObject.SpawnHoldableObject(_controller.burnSO, _controller);
            _controller.SetState(_controller._noneState);
        }
    }

    public override void Exit()
    {
        _currentTime = 0f;
        _controller.progressBar.ResetBar();
    }
}