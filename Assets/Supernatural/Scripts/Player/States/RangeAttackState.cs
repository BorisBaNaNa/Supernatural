using System.Collections;
using UnityEngine;

public class RangeAttackState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;
    private RangeAttack rangeAttack;

    public RangeAttackState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
        rangeAttack = _player.GetComponent<RangeAttack>();
    }

    public void Enter()
    {
        if (rangeAttack == null)
            return;
        _stateMachine.ThisStateIsActive = true;

        if (rangeAttack.Fire())
        {
            _player.AnimController.PlayOneTimeAnimation(this);
            SoundManager.PlaySfx(_player.rangeAttackSound);
        }

        _player.StartCoroutine(SwichActiveStateRuotime(_player.AnimController.RangeAttackAnim.Animation.Duration + 0.1f));
    }

    public void Exit() { }

    private IEnumerator SwichActiveStateRuotime(float time = -1)
    {
        if (time < 0) time = _player.MinActiveStateTime;

        yield return new WaitForSeconds(time);
        _stateMachine.ThisStateIsActive = false;
    }
}
