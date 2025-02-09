public class IdleState : IState
{
    private readonly PlayerStateMachine _stateMachine;

    public IdleState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Player.AnimController.PlayStableAnimation(this);
    }

    public void Exit() { }
}
