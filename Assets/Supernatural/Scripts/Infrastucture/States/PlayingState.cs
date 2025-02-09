using System.Collections;
using UnityEngine;

public class PlayingState : IGameState
{
    private GameStateMachine _stateSwitcher;

    public PlayingState(GameStateMachine stateSwitcher)
    {
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        AllServices.Instance.GetService<LevelManager>().StartTimer();
        AllServices.Instance.GetService<LevelManager>().StartCoroutine(EnableInput());
    }

    public void Exit()
    {
        AllServices.Instance.GetService<LevelManager>().Player.Inputs.Player.Disable();
    }

    private IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.2f);
        AllServices.Instance.GetService<LevelManager>().Player.Inputs.Player.Enable();
    }
}
