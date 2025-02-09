using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GameFinish : MonoBehaviour
{
    public Text scoreText;
    public Text timerText;
    public GameObject Buttons;
    public GameObject Next;
    public AudioClip soundCouting;

    public float countingSpeed = 2f;
    int score;
    int timer;

    void Awake()
    {
        Buttons.SetActive(false);
        if (LevelManager.IsLastLevel)
            Next.SetActive(false);      //dont show the next button when this is the final level
    }

    // Use this for initialization
    void Start()
    {
        timer = LevelManager.CurrentTime_;
        score = LevelManager.Point_;
        StartCoroutine(Counting());
    }

    IEnumerator Counting()
    {
        while (timer > 0)
        {
            yield return new WaitForSecondsRealtime(countingSpeed * Time.deltaTime);

            timer--;
            score++;
            timerText.text = timer.ToString("000");
            scoreText.text = score.ToString("0000000");
            SoundManager.PlaySfx(soundCouting);
        }

        Buttons.SetActive(true);
    }
}
