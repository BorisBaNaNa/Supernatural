using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateMachine : IStateSwitcher
{
    public readonly Player Player;
    public Action CurrentAction;
    public Action Jump;
    public bool ThisStateIsActive;

    public bool ThisStateIsEnd => _currentState is DeathState || _currentState is FinishState;

    private readonly List<IState> _states;
    private IState _currentState;

    public PlayerStateMachine(Player player)
    {
        Player = player;

        _states = new List<IState>
        {                               // тип переключения     Слой анимирования
            new IdleState       (this), //      автомат                    0
            new WalkState       (this), //      автомат                    0
            new CrouchState     (this), //      автомат                    0
            new JumpState       (this), //      ручной                     0
            new FallState       (this), //      автомат                    0
            new LandState       (this), //      автомат                    0
            new SlideState      (this), //      автомат                    0
            new MelleAttackState(this), //      ручной                     1
            new RangeAttackState(this), //      ручной                     1
            new TakeDamageState (this), //      ручной                     1
            new DeathState      (this), //      ручной                     0
            new FinishState     (this), //      ручной                     0
            new RespawnState    (this), //      ручной                     0
        };
    }

    public void StateSwitch<TState>() where TState : IState
    {
        IState newState = _states.FirstOrDefault(state => state is TState);
        if (ThisStateIsEnd && newState is not RespawnState)
            return;

        if (ThisStateIsActive && newState is not DeathState/* && IsAttackState(newState)*/)
            return;

        if (_currentState is SlideState && IsAttackState(newState))
            return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void StateControl()
    {
        if (ThisStateIsEnd)
            return;

        if (Player.IsGrounded && !Player.WasGrounded && Player.IsHardLand)
            TrySetState<LandState>();
        else if (Mathf.Abs(Player.Velocity.x) < 1 && Player.Velocity.y == 0)
        {
            if (Player.MoveDir == Vector2.zero)
                TrySetState<IdleState>();
            else
                TrySetState<CrouchState>(Player.MoveDir.y < 0);
        }
        else if (Player.Velocity.y == 0 && Player.IsGrounded)
            TrySetState<WalkState>(Player.MoveDir.x != 0);
        else if (Player.IsSliding)
            TrySetState<SlideState>();
        else if (_currentState is not CrouchState)
            TrySetState<FallState>();

        UpdatePlayerStateStr();
    }

    private void UpdatePlayerStateStr() => Player.CurrentState = _currentState.GetType().Name;

    private bool IsAttackState(IState state) => state is MelleAttackState || state is RangeAttackState;

    private void TrySetState<NewState>(bool condition = true) where NewState : IState
    {
        if (condition && _currentState is not NewState)
            StateSwitch<NewState>();
    }
}
