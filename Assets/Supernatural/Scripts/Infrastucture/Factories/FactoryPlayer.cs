using System.Collections.Generic;
using UnityEngine;

public class FactoryPlayer : IService
{
    private readonly List<Player> _playerPrefabs;
    private Player _spawnPlayerPrefab;

    public FactoryPlayer(IEnumerable<Player> playerPrefabs)
    {
        _playerPrefabs = new(playerPrefabs);
        _spawnPlayerPrefab = _playerPrefabs[0];
    }

    public static void SetSpawnPlayer(int InstanceID)
    {
        if (InstanceID == 0)
            return;

        FactoryPlayer factoryPlayer = AllServices.Instance.GetService<FactoryPlayer>();

        foreach (Player player in factoryPlayer._playerPrefabs)
            if (player.gameObject.GetInstanceID() == InstanceID)
                factoryPlayer._spawnPlayerPrefab = player;
    }

    public Player BuildPlayer(Vector3 at) => Object.Instantiate(_spawnPlayerPrefab, at, Quaternion.identity);

    public Player BuildPlayer(Transform at) => Object.Instantiate(_spawnPlayerPrefab, at.position, at.rotation);
}
