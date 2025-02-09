using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour, IPlayerRespawnListener
{
    public int coinToAdd = 1;
    public int pointToAdd = 25;
    public GameObject Effect;
    public bool isRespawnCheckPoint = true;
    public AudioClip sound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() == null)
            return;

        SoundManager.PlaySfx(sound);

        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        levelManager.Coin += coinToAdd;
        levelManager.Point += pointToAdd;

        if (Effect != null)
            Instantiate(Effect, transform.position, transform.rotation);

        if (pointToAdd != 0)
            AllServices.Instance.GetService<FactoryFloatText>().BuildFloatingText(transform.position, pointToAdd.ToString(), Color.white);

        gameObject.SetActive(false);
    }

    public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
    {
        if (isRespawnCheckPoint)
            gameObject.SetActive(true);
    }
}
