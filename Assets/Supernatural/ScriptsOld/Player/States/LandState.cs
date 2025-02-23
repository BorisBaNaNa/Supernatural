public class LandState : IState //add hard land
{
    private PlayerStateMachine _stateMachine;

    public LandState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
        // Start sound
    }

    public void Exit() { }
}
