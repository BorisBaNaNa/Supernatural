public class FallState : IState
{
    private readonly PlayerStateMachine _stateMachine;

    public FallState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Player.AnimController.PlayStableAnimation(this);
    }

    public void Exit() { }
}
