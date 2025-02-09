public class FinishState : IState
{
    private PlayerStateMachine _stateMachine;

    public FinishState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        AllServices.Instance.GetService<GameManager>().GameFinish();

        _stateMachine.Player.Inputs.Player.Disable();
        _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
    }

    public void Exit() { }
}
