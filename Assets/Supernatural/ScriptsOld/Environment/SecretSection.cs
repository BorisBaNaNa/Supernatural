using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretSection : MonoBehaviour
{
    [SerializeField]
    private GameObject _falseForeground;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        _falseForeground.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        _falseForeground.SetActive(true);
    }
}
