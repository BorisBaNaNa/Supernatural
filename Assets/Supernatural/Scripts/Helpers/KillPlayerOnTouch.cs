using UnityEngine;
using System.Collections;

public class KillPlayerOnTouch : MonoBehaviour
{
    public bool killEnemies = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();

        if (player != null)
            player.Kill();
        else if (killEnemies && other.gameObject.GetComponent(typeof(ICanTakeDamage)))
            other.gameObject.SetActive(false);
    }
}