using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public GameObject NotEnoughCoinsPanel;
    public int LivePrice;
    public int BulletPrice;

    public AudioClip BoughtSound;

    public Text LivePriceTxt;
    public Text BulletPriceTxt;

    public Text LivesTxt;
    public Text BulletTxt;
    public Text CoinTxt;

    private GameManager _gameManager;
    private void Awake()
    {
        _gameManager = AllServices.Instance.GetService<GameManager>();
    }

    void Start()
    {
        LivePriceTxt.text = LivePrice.ToString();
        BulletPriceTxt.text = BulletPrice.ToString();
        CoinTxt.text = _gameManager.Coin.ToString();
    }

    void Update()
    {
        LivesTxt.text = "Live: " + _gameManager.LivesCount;
        BulletTxt.text = "Bullet: " + _gameManager.Bullet;
        CoinTxt.text = _gameManager.Coin.ToString();
    }

    public void BuyLive()
    {
        if (_gameManager.Coin >= LivePrice)
        {
            _gameManager.Coin -= LivePrice;
            _gameManager.LivesCount++;
            SoundManager.PlaySfx(BoughtSound);
        }
        else
            NotEnoughCoinsPanel.SetActive(true);
    }

    public void BuyBullet()
    {
        if (_gameManager.Coin >= BulletPrice)
        {
            _gameManager.Coin -= BulletPrice;
            _gameManager.Bullet++;
            SoundManager.PlaySfx(BoughtSound);
        }
        else
            NotEnoughCoinsPanel.SetActive(true);
    }
}
