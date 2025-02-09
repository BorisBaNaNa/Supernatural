using UnityEngine;

internal class BoostraperState : IGameState
{
    readonly GameStateMachine _stateMachine;

    public BoostraperState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }
}
