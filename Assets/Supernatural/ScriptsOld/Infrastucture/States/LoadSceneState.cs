using UnityEngine.SceneManagement;

internal class LoadSceneState : IGameState
{
    readonly GameStateMachine _stateMachine;

    public LoadSceneState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void Exit()
    {

    }

    public static void LoadScene(string sceneName, bool LoadOnClick = false)
    {
        GameManager.LoadSceneName = sceneName;
        GameManager.LoadOnClick = LoadOnClick;
        GameManager.SwichGameState<LoadSceneState>();
    }
}
