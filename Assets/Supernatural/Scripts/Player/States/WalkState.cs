public class WalkState : IState
{
    private readonly PlayerStateMachine _stateMachine;

    public WalkState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Player.AnimController.PlayStableAnimation(this);
    }

    public void Exit() { }
}
