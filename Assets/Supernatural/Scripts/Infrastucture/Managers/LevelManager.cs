using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;

public class LevelManager : MonoBehaviour, IService
{
    public static bool IsBossLevel => AllServices.Instance.GetService<LevelManager>().isBossLevel;
    public static bool IsLastLevel => AllServices.Instance.GetService<LevelManager>().NextLevelName == string.Empty;
    public static int CurrentTime_ => AllServices.Instance.GetService<LevelManager>()._currentTime;
    public static int Point_ => AllServices.Instance.GetService<LevelManager>().Point;
    public int CurrentTime => _currentTime;
    public int BulletCount
    {
        get => _playerRangeAttack.BulletCount;
        set => _playerRangeAttack.BulletCount = value;
    }

    public Player Player { get; set; }
    public int Point { get; set; }
    public int Coin { get; set; }

    public CinemachineVirtualCamera Camera;

    [Header("Level Paremeter")]
    public string LevelName = "World 01-01";
    public string NextLevelName;

    [Header("Level settings")]
    public AudioClip MusicsGame;
    public List<CheckPoint> Checkpoints;
    public int PlayTime = 120;
    public int AlarmTimeLess = 60;
    public bool IsTrain;

    [Header("Level sound")]
    public AudioClip SoundCheckpoint;
    public AudioClip SoundTimeLess;
    public AudioClip SoundTimeUp;

    [SerializeField]
    private bool isBossLevel = false;
    [SerializeField]
    private Transform _floatingTextParent;

    private Coroutine _timer;
    private RangeAttack _playerRangeAttack;
    private int _currentCheckpointIndex = -1;
    private int _currentTime;
    private int _saveTimerCheckPoint;
    private int _savePointCheckPoint;
    private int _saveCoinCheckPoint;
    private int _saveBulletCheckPoint;

    public void Awake()
    {
        Initialize();
        GameManager.SwichGameState<LoadLevelRecourceState>();
        SetupVals();
        Player.Inputs.Player.Disable();
        SoundManager.PlayMusic(MusicsGame);
    }

    public void Start()
    {
        AssignCheckPoints();
    }

    public void Update()
    {
        SaveInfoOnCheckPoint();
    }

    public void InitializeManager(Player player)
    {
        Player = player;
    }

    public void PlayerWasKilled()
    {
        Camera.Follow = null;
        Camera.LookAt = null;
    }

    public void GotoCheckPoint()
    {
        LoadResourceInfo();
        Checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
        Camera.Follow = Player.transform;
        Camera.LookAt = Player.transform;
        GameManager.SwichGameState<PlayingState>();
    }

    public void StartTimer()
    {
        if (PlayTime > 0)
            _timer = StartCoroutine(CountDownTimer());
    }

    public void StopTimer()
    {
        if (_timer != null)
            StopCoroutine(_timer);
    }

    private void Initialize()
    {
        AllServices.Instance.RegisterService(this);
        AllServices.Instance.GetService<FactoryFloatText>().SetTextParent(_floatingTextParent);

        if (Camera == null)
            Camera = FindObjectOfType<CinemachineVirtualCamera>();
        CheckpointsInit();
    }

    private void CheckpointsInit()
    {
        if (Checkpoints.Count == 0)
            return;

        Checkpoints.Sort(point => point.transform.position.x);
        _currentCheckpointIndex = 0;
    }

    private void SetupVals()
    {
        _playerRangeAttack = Player.GetComponent<RangeAttack>();
        _currentTime = PlayTime;
        _saveTimerCheckPoint = PlayTime;
        SaveResourceInfo();

        if (IsTrain)
        {
            Player.GodMode = true;
        }
    }

    private void SaveInfoOnCheckPoint()
    {
        if (_currentCheckpointIndex + 1 >= Checkpoints.Count)
            return;

        var distanceToNextCheckPoint = Checkpoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckPoint >= 0)
            return;

        _currentCheckpointIndex++;
        SaveResourceInfo();

        SoundManager.PlaySfx(SoundCheckpoint);
    }

    private void SaveResourceInfo()
    {
        _saveTimerCheckPoint = _currentTime;
        _savePointCheckPoint = Point;
        _saveCoinCheckPoint = Coin;
        _saveBulletCheckPoint = BulletCount;
    }

    private void LoadResourceInfo()
    {
        _currentTime = _saveTimerCheckPoint;
        Point = _savePointCheckPoint;
        Coin = _saveCoinCheckPoint;
        BulletCount = _saveBulletCheckPoint;
    }

    private void AssignCheckPoints()
    {
        IEnumerable<IPlayerRespawnListener> listener = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (IPlayerRespawnListener _listener in listener)
        {
            for (int i = Checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)_listener).transform.position.x - Checkpoints[i].transform.position.x;
                if (distance >= 0)
                {
                    Checkpoints[i].AssignObjectToCheckPoint(_listener);
                    break;
                }
            }
        }
    }

    private IEnumerator CountDownTimer()
    {
        while (_currentTime > 0)
        {
            yield return new WaitForSeconds(1);

            _currentTime--;
            if (_currentTime == AlarmTimeLess)
                SoundManager.PlaySfx(SoundTimeLess);
            else if (_currentTime <= 0)
            {
                SoundManager.PlaySfx(SoundTimeUp);
                Player.Kill();
            }
        }
    }
}
