using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;

    public RespawnState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
    }

    public void Enter()
    {
        _player.Health = _player.maxHealth;

        //EnableColliders();
        //_player.Controller.HandlePhysic = true;

        _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
        _player.StartCoroutine(StartPaying());
    }

    private void EnableColliders()
    {
        var boxCo = _player.GetComponents<BoxCollider2D>();
        foreach (var box in boxCo)
        {
            box.enabled = true;
        }
        var CirCo = _player.GetComponents<CircleCollider2D>();
        foreach (var cir in CirCo)
        {
            cir.enabled = true;
        }
    }

    public void Exit() { }

    private IEnumerator StartPaying()
    {
        yield return new WaitForSeconds(0.5f);
        _player.Inputs.Player.Enable();
        _stateMachine.StateSwitch<IdleState>();
        _player.GetComponent<Collider2D>().enabled = true;
    }
}
