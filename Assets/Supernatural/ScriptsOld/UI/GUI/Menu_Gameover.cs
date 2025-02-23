using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_Gameover : MonoBehaviour
{
    public Text liveText;
    public GameObject Next;
    public GameObject StartOver;
    public GameObject Buttons;
    int lives;

    void Awake()
    {
        if (LevelManager.IsLastLevel)
            Next.SetActive(false);      //dont show the next button when this is the final level
    }

    void OnEnable()
    {

        Buttons.SetActive(false);

        if (!AllServices.Instance.GetService<GameManager>().IsHaveNotLives)
            lives = AllServices.Instance.GetService<GameManager>().LivesCount;
        else
            lives = 0;

        // Сделать доступность уровней по прохождении
        //var levelReached = PlayerPrefs.GetInt (GlobalValue.worldPlaying.ToString (), 1);

        //if (GlobalValue.levelPlaying < levelReached)
        //	Next.SetActive (true);
        //else
        //	Next.SetActive (false);

        liveText.text = (lives + 1).ToString("00");
        StartCoroutine(SubtractLiveCo(1));
    }

    IEnumerator SubtractLiveCo(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        liveText.text = lives.ToString("00");
        liveText.gameObject.GetComponent<Animator>()?.SetTrigger("live");

        StartOver.SetActive(lives <= 0);
        Buttons.SetActive(!(lives <= 0));
    }
}
