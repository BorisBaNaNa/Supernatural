using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boostraper : MonoBehaviour
{
    [SerializeField]
    private List<Player> _playerPrefabs;
    [SerializeField]
    private Projectile _projectilePrefab;
    [SerializeField]
    private SoundManager _soundManagerPrefab;
    [SerializeField]
    private GameManager _gameManagerPrefab;
    [SerializeField]
    private FloatingText _floatingTextPrefab;

    private GameStateMachine _stateMachine;
    private GameManager _gameManager;


    public void Awake()
    {
        if (AllServices.Instance.GetService<GameManager>() != null)
        {
            Debug.Log("Resourses already initialized! Boostraper was destroyed...");
            Destroy(gameObject);
            return;
        }

        InitializeServices();
        InitializeGame();
        _stateMachine.StateSwitch<BoostraperState>();
    }

    private void InitializeGame()
    {
        _stateMachine = new GameStateMachine();

        _gameManager = Instantiate(_gameManagerPrefab);
        _gameManager.Initialize(_stateMachine);
        AllServices.Instance.RegisterService(_gameManager);

        SoundManager soundManager = Instantiate(_soundManagerPrefab);
        soundManager.Initialize();
        AllServices.Instance.RegisterService(soundManager);
    }

    private void InitializeServices()
    {
        AllServices.Instance.RegisterService(new FactoryPlayer(_playerPrefabs));
        FactoryPlayer.SetSpawnPlayer(SaveInfoManager.LoadChoosenCharacterID());

        AllServices.Instance.RegisterService(new FactoryProjectile(_projectilePrefab));
        AllServices.Instance.RegisterService(new FactoryFloatText(_floatingTextPrefab));
    }
}
