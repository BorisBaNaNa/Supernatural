using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUI : MonoBehaviour
{

    public bool OnlyController = false;

    public Text scoreText;
    public Text liveText;
    public Text bulletText;
    public Text coinText;
    public Text timerText;
    public GameObject UIPlayerController;

    private LevelManager _levelManager;
    private bool _timerOff = false;

    private void Awake()
    {
        #if UNITY_STANDALONE
        UIPlayerController.SetActive(false);
        #elif UNITY_ANDROID
			UIPlayerController.SetActive(true);
        #endif

        _levelManager = AllServices.Instance.GetService<LevelManager>();
        if (_levelManager != null)
        {
            _timerOff = _levelManager.PlayTime <= 0;
            if (_timerOff)
                timerText.transform.parent.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OnlyController) return;

        if (!_timerOff)
            timerText.text = _levelManager.CurrentTime.ToString("000");

        scoreText.text = _levelManager.Point.ToString("0000000");
        coinText.text = _levelManager.Coin.ToString("00");
        bulletText.text = _levelManager.BulletCount.ToString();
        liveText.text = "x" + AllServices.Instance.GetService<GameManager>().LivesCount.ToString();
    }
}
