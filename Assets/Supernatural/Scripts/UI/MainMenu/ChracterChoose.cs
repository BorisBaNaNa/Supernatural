using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChracterChoose : MonoBehaviour
{
    public bool UnlockDefault = false;
    public int Price;

    public GameObject CharacterPrefab;
    public GameObject UnlockButton;
    public GameObject NotEnoughCoinsPanel;

    public Text PriceTxt;
    public Text StateTxt;

    private SoundManager _soundManager;
    private bool _isUnlock;
    private int _characterID;

    // Use this for initialization
    void Start()
    {
        _soundManager = AllServices.Instance.GetService<SoundManager>();
        _characterID = CharacterPrefab.GetInstanceID();

        if (UnlockDefault)
            _isUnlock = true;
        else
            _isUnlock = SaveInfoManager.LoadStateCharacterID(_characterID);

        UnlockButton.SetActive(!_isUnlock);
        PriceTxt.text = Price.ToString();
    }

    void Update()
    {
        if (!_isUnlock)
            return;

        StateTextUpdate();
    }

    public void Unlock()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        GameManager gameManager = AllServices.Instance.GetService<GameManager>();

        if (gameManager.Coin >= Price)
        {
            gameManager.Coin -= Price;
            //Unlock
           SaveInfoManager.SaveStateCharacterID(_characterID, 1);

            _isUnlock = true;
            UnlockButton.SetActive(false);
        }
        else
            NotEnoughCoinsPanel.SetActive(true);
    }

    public void Pick()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);

        if (!_isUnlock)
        {
            Unlock();
            return;
        }

        GameManager.ChoosenCharacterID = _characterID;
        FactoryPlayer.SetSpawnPlayer(_characterID);
    }


    private void StateTextUpdate() =>
        StateTxt.text = GameManager.ChoosenCharacterID == _characterID ? "Picked" : "Choose";
}
