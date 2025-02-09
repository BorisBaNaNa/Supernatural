using UnityEngine;
using System.Collections;

public class GiveHealth : MonoBehaviour, IPlayerRespawnListener
{
    public int healthToGive;
    public GameObject Effect;
    public bool isRespawnCheckPoint = true;
    public bool IsTrain;
    public AudioClip soundEffect;

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        var Player = other.gameObject.GetComponent<Player>();
        if (Player == null)
            return;

        Player.GiveHealth(healthToGive, gameObject);
        if (Effect != null)
            Instantiate(Effect, transform.position, Quaternion.identity);

        SoundManager.PlaySfx(soundEffect);

        gameObject.SetActive(IsTrain);
    }

    #region IPlayerRespawnListener implementation

    public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
    {
        if (isRespawnCheckPoint)
            gameObject.SetActive(true);
    }

    #endregion
}

