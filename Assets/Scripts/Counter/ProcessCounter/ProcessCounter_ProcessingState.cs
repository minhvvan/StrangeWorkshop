using UnityEngine;

public class ProcessCounter_ProcessingState : BaseState<ProcessCounter>
{
    public ProcessCounter_ProcessingState(ProcessCounter controller) : base(controller) {}
    
    private float _currentTime;
    private GameObject _smokingEff;
    public override void Enter()
    {
        //UI - SetActive ture
        _controller.progressBar.gameObject.SetActive(true);
        _controller.progressBar.SetBar(_controller.currentRecipe.processTime);
        _smokingEff = VFXManager.Instance.TriggerVFX(VFXType.PROCESSCOUNTERWORKING, _controller.transform,
            rotation: Quaternion.Euler(-90f, 0, 0), returnAutomatically: false);
    }

    public override void UpdateState()
    {
        _currentTime += Time.deltaTime;
        _controller.progressBar.UpdateProgressBar(_currentTime);
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
        _controller.progressBar.ResetBar();
        _controller.currentRecipe = null;
        _controller.isWork = false;
        //UI - SetActive false
        VFXManager.Instance.ReturnVFX(VFXType.PROCESSCOUNTERWORKING, _smokingEff);
    }
}