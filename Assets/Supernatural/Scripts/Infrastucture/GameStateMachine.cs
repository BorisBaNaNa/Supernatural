using Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class GameStateMachine : IStateSwitcher  
{
    public string CurrentState => _currentState.GetType().ToString();

    private readonly List<IGameState> _states;
    private IGameState _currentState;
    public GameStateMachine()
    {
        _states = new List<IGameState>
        {
            new BoostraperState(this),
            new MainMenuState(this),
            new LoadLevelRecourceState(this),
            new PlayingState(this),
            new PausedState(this),
            new LoadSceneState(this),
        };
    }

    public void StateSwitch<TState>() where TState : IState
    {
        _currentState?.Exit();
        _currentState = _states.FirstOrDefault(state => state is TState);
        _currentState.Enter();
    }
}