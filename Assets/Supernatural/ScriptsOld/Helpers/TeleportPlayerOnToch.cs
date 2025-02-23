using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayerOnToch : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();

        if (player == null)
            return;
        
        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        player.RespawnAt(levelManager.Checkpoints[0].transform.position);
    }
}
