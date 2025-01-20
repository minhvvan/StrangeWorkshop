using UnityEngine;

public class Character_DashState : BaseState<SampleCharacterController>
{
    public Character_DashState(SampleCharacterController controller) : base(controller) { }
    private float dashTimer;
    public override void Enter()
    {
        dashTimer = 0f;
        Debug.Log("Dash Enter");
    }

    public override void Exit()
    {
        Debug.Log("Dash Exit");
    }

    public override void UpdateState()
    {
        dashTimer += Time.deltaTime;
        
        if(dashTimer >= _controller.dashAccelTime + _controller.dashDecelTime)
        {
            _controller.SetState(_controller.runState);
        }
    }
}