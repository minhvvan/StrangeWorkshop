using UnityEngine;

public class CollectingBot_CollectingState : BaseState<CollectingBot>
{
    public CollectingBot_CollectingState(CollectingBot controller) : base(controller){}

    private float collectingTime = 2f;
    private float currentTime= 0f;

    public override void Enter()
    {
        Debug.Log("CollectingBot Enter");
    }

    public override void UpdateState()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= collectingTime)
        {
            if (_controller.target.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                baseCounter.Interact(_controller);
            }

            // 베리어로
            // EnemyPathfinder.Instance.MatchTarget 이용하면 될 듯
            _controller.target = EnemyPathfinder.instance.MatchTarget(_controller.gameObject.transform);
            _controller.SetState(_controller._chaseState);
        }
    }

    public override void Exit()
    {
        currentTime = 0f;
    }
}