
public class CrouchState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;

    public CrouchState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
    }

    public void Enter()
    {
        _player.Controller.Crouch(true);
        _player.AnimController.PlayStableAnimation(this);
    }

    public void Exit()
    {
        _player.Controller.Crouch(false);
        _player.AnimController.PlayExitStateAnimation(this);
    }
}
