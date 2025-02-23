using UnityEngine;

public class PausedState : IGameState
{
    private IStateSwitcher _stateSwitcher;

    public PausedState(IStateSwitcher stateSwitcher)
    {
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        AllServices.Instance.GetService<LevelManager>().StopTimer();
        Time.timeScale = 0;
    }

    public void Exit()
    {
        Time.timeScale = 1;
    }
}
