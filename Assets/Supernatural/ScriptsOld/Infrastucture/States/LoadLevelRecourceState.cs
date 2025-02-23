using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

internal class LoadLevelRecourceState : IGameState
{
    private IStateSwitcher _stateSwitcher;
    private FactoryPlayer _factoryPlayer;
    CinemachineVirtualCamera _virtualCamera;

    public LoadLevelRecourceState(IStateSwitcher stateSwitcher)
    {
        _factoryPlayer = AllServices.Instance.GetService<FactoryPlayer>();

        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        Player player = CreatePlayer();
        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();

        levelManager.InitializeManager(player);
        SetupVirtualCamera(player, levelManager);

        GameManager.SwichGameState<PausedState>();
    }

    public void Exit()
    {
    }

    private Player CreatePlayer()
    {
        Vector3 spawnPoint = GameObject.FindGameObjectWithTag("LevelStart").transform.position;
        return _factoryPlayer.BuildPlayer(spawnPoint);
    }

    private void SetupVirtualCamera(Player player, LevelManager levelManager)
    {
        _virtualCamera = levelManager.Camera;
        _virtualCamera.Follow = player.transform;
        _virtualCamera.LookAt = player.transform;
    }

}
