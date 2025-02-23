using System.Collections;
using UnityEngine;

public class TakeDamageState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;

    public TakeDamageState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
    }

    public void Enter()
    {
        _stateMachine.ThisStateIsActive = true;
        SoundManager.PlaySfx(_player.hurtSound);

        if (_player.HurtEffect != null)
            Object.Instantiate(_player.HurtEffect, _player.LastInstigator.transform.position, Quaternion.identity);

        _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
        // Debug.LogWarning("Player enter in take damage state");
        _player.StartCoroutine(SwichActiveStateRuotime());
    }

    private IEnumerator SwichActiveStateRuotime(float time = -1)
    {
        if (time < 0) time = _player.MinActiveStateTime;

        yield return new WaitForSeconds(time);
        _stateMachine.ThisStateIsActive = false;
    }

    public void Exit() { }
}
