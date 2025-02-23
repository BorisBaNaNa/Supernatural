using UnityEngine;

public class DeathState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;

    public DeathState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
    }

    public void Enter()
    {
        _player.AnimController.PlayOneTimeAnimation(this);
        _player.Inputs.Player.Disable();
        _player.MoveDir = Vector2.zero;
        SoundManager.PlaySfx(_player.deadSound);
        _player.GetComponent<Collider2D>().enabled = false;

        _player.SetForce(new Vector2(0, 7f));
        _player.Health = 0;
        //_player.Controller.HandlePhysic = false;
        AllServices.Instance.GetService<GameManager>().GameOver();

    }

    public void Exit() { }
}
