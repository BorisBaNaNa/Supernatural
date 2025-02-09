using System.Collections;
using UnityEngine;

public class MelleAttackState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;
    protected MeleeAttack meleeAttack;

    public MelleAttackState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
        meleeAttack = _player.GetComponent<MeleeAttack>();
    }

    public void Enter()
    {
        if (meleeAttack == null || !meleeAttack.Attack())
            return;

        _stateMachine.ThisStateIsActive = true;

        _player.AnimController.PlayOneTimeAnimation(this);
        SoundManager.PlaySfx(_player.meleeAttackSound);

        _player.StartCoroutine(SwichActiveStateRuotime(_player.AnimController.MeleeAttackAnim.Animation.Duration + 0.1f));
    }

    public void Exit() { }

    private IEnumerator SwichActiveStateRuotime(float time = -1)
    {
        if (time < 0) time = _player.MinActiveStateTime;

        yield return new WaitForSeconds(time);
        _stateMachine.ThisStateIsActive = false;
    }
}
