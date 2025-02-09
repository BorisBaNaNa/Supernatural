public interface IState
{
    void Enter();

    void Exit();
}

public interface IGameState : IState { }

public interface IPlayerState : IState { }