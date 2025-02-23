using UnityEngine;
using System.Collections;

public class GiveBullet : MonoBehaviour, IPlayerRespawnListener
{
    public int bulletToAdd = 1;
    public int pointToAdd = 5;
    public GameObject Effect;
    public bool isRespawnCheckPoint = true;
    public bool IsTrain;
    public AudioClip sound;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() == null)
            return;

        SoundManager.PlaySfx(sound);

        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        levelManager.BulletCount += bulletToAdd;
        levelManager.Point += pointToAdd;

        if (Effect != null)
            Instantiate(Effect, transform.position, transform.rotation);

        gameObject.SetActive(IsTrain);
    }

    public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
    {
        if (isRespawnCheckPoint)
            gameObject.SetActive(true);
    }
}
