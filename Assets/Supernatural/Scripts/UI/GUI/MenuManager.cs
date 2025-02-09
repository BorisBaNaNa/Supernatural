using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour, IService
{
    public GameObject StartMenu;
    public GameObject GUI;
    public GameObject GameoverMenu;
    public GameObject StarnOnClickMenu;
    public GameObject GameFinishMenu;
    public GameObject GamePauseMenu;

    [SerializeField]
    private bool StartAfterClick;

    private SoundManager _soundManager;

    void Awake()
    {
        Init();
        ActivateUI();
    }

    // Use this for initialization
    void Start()
    {
        if (!StartAfterClick)
            StartGameWithDalay(2f);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        SoundManager.PlaySfx(_soundManager.SoundClick);

        // Добавить больше уровней
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SoundManager.PlaySfx(_soundManager.SoundClick);

        LoadSceneState.LoadScene(SceneManager.GetActiveScene().name, false);
    }

    public void HomeScene()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);

        LoadSceneState.LoadScene("MainMenu", false);
    }

    public void GameFinish()
    {
        StartCoroutine(GamefinishCo(2));
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCo(1.5f));
    }

    public void Pause()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        if (Time.timeScale == 0)
        {
            GamePauseMenu.SetActive(false);
            GUI.SetActive(true);
            GameManager.SwichGameState<PlayingState>();
        }
        else
        {
            GamePauseMenu.SetActive(true);
            GUI.SetActive(false);
            GameManager.SwichGameState<PausedState>();
        }
    }

    public void GotoCheckPoint()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        GUI.SetActive(true);
        GameoverMenu.SetActive(false);

        if (!LevelManager.IsBossLevel)
            AllServices.Instance.GetService<LevelManager>().GotoCheckPoint();
        else
            RestartGame();
    }

    public void StartGameWithDalay(float delay) => StartCoroutine(StartGame(delay));

    private void Init()
    {
        AllServices.Instance.RegisterService(this);
        _soundManager = AllServices.Instance.GetService<SoundManager>();
    }

    private void ActivateUI()
    {
        StarnOnClickMenu.SetActive(StartAfterClick);
        StartMenu.SetActive(!StartAfterClick);
        GUI.SetActive(false);
        GameoverMenu.SetActive(false);
        GameFinishMenu.SetActive(false);
        GamePauseMenu.SetActive(false);
    }

    private IEnumerator StartGame(float time)
    {
        yield return new WaitForSecondsRealtime(time - 0.5f);
        StartMenu.GetComponent<Animator>().SetTrigger("play");

        yield return new WaitForSecondsRealtime(0.5f);
        StartMenu.SetActive(false);
        GUI.SetActive(true);

        AllServices.Instance.GetService<GameManager>().StartGame();
    }

    private IEnumerator GamefinishCo(float time)
    {
        GUI.SetActive(false);

        yield return new WaitForSecondsRealtime(time);

        GameManager.SwichGameState<PausedState>();
        GameFinishMenu.SetActive(true);
    }

    private IEnumerator GameOverCo(float time)
    {
        GUI.SetActive(false);

        yield return new WaitForSecondsRealtime(time);

        GameManager.SwichGameState<PausedState>();
        GameoverMenu.SetActive(true);
    }
}
