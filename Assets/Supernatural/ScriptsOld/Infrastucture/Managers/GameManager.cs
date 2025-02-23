using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IService
{
    public static int WorldPlaying { get; set; } = 0;
    public static int LevelPlaying { get; set; } = 0;
    public static string LoadSceneName { get; set; }
    public static bool LoadOnClick { get; set; } = true;
    public static int ChoosenCharacterID
    {
        get => _choosenCharacterID;

        set
        {
            _choosenCharacterID = value;
            SaveInfoManager.SaveChoosenCharacterID(ChoosenCharacterID);
        }
    }

    public GameStateMachine StateMachine { get; private set; }
    public int Coin 
    { 
        get => _coin; 
        set
        {
            _coin = value;
            SaveInfoManager.SaveCoins(_coin);
        }
    }
    public int LivesCount
    {
        get => _livesCount;
        set
        {
            _livesCount = value;
            SaveInfoManager.SaveLives(_livesCount);
        }
    }
    public int Bullet
    {
        get => _bullet;
        set
        {
            _bullet = value;
            SaveInfoManager.SaveBullets(_bullet);
        }
    }

    public bool IsHaveNotLives;
    public string CurrentGameState;

    [SerializeField]
    private int _defaultCoinCount = 25;
    [SerializeField]
    private int _defaultBulletCount = 15;
    [SerializeField]
    private int _defaultLivesCount = 10;

    private static int _choosenCharacterID = -1;
    private int _coin;
    private int _bullet;
    private int _livesCount;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetupValues();
    }

    public void Update()
    {
        CurrentGameState = StateMachine.CurrentState;
    }

    public void Initialize(GameStateMachine gameStateMachine)
    {
        StateMachine = gameStateMachine;
    }

    public void StartGame()
    {
        SwichGameState<PlayingState>();
        IsHaveNotLives = false;
    }

    public void GameOver()
    {
        AllServices.Instance.GetService<LevelManager>().PlayerWasKilled();
        AllServices.Instance.GetService<MenuManager>().GameOver();
        SoundManager.PlaySfx(AllServices.Instance.GetService<SoundManager>().SoundGameover);

        if (--_livesCount <= 0)
        {
            // Добавить механику постепенного восстановления жизней
            SetupValues();
            IsHaveNotLives = true;
        }
    }

    public void GameFinish()
    {
        AllServices.Instance.GetService<MenuManager>().GameFinish();
        SoundManager.PlaySfx(AllServices.Instance.GetService<SoundManager>().SoundGamefinish);

        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        _coin += levelManager.Coin;

        // Добавить разблокировку уровней
    }

    public static void SwichGameState<TState>() where TState : IGameState =>
        AllServices.Instance.GetService<GameManager>().StateMachine.StateSwitch<TState>();

    public void SetupValues()
    {
        _coin = SaveInfoManager.LoadCoins(_defaultCoinCount);
        _bullet = SaveInfoManager.LoadBullets(_defaultBulletCount);
        _livesCount = SaveInfoManager.LoadLives(_defaultLivesCount);
    }
}
