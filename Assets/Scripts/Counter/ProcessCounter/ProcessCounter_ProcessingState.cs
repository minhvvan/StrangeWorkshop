using UnityEngine;

public class ProcessCounter_ProcessingState : BaseState<ProcessCounter>
{
    public ProcessCounter_ProcessingState(ProcessCounter controller) : base(controller) {}
    
    private float _currentTime;

    public override void Enter()
    {
        //UI - SetActive ture
    }

    public override void UpdateState()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _controller.currentRecipe.processTime)
        {
            _controller.ClearHoldableObject();
            HoldableObject.SpawnHoldableObject(_controller.currentRecipe.output, _controller);
            _controller.SetState(_controller._overState);
        }
    }

    public override void Exit()
    {
        _currentTime = 0;
        //UI - SetActive false
    }
}