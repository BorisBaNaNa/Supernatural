using UnityEngine;
using System.Collections;

public class GameFinishFlag : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player;
        if ((player = other.gameObject.GetComponent<Player>()) == null)
            return;

        player.GameFinish();
    }
}
