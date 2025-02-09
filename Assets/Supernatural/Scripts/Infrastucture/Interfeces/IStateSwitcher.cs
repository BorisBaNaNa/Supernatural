public interface IStateSwitcher
{
    void StateSwitch<TState>() where TState : IState;
}