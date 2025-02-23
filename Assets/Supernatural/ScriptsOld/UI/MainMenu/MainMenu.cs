using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("MenuPanels")]
    public GameObject StartMenu;
    public GameObject WorldsChoose;
    public GameObject LevelsChoose;
    public GameObject Shop;
    public GameObject HelperPanel;
    public GameObject[] WorldLevel;

    [Header("SoundPannelsSettings")]
    public Image SoundImage;
    public Slider SoundSlider;
    public Sprite SoundOn;
    public Sprite SoundOff;

    public Image MusicImage;
    public Slider MusicSlider;
    public Sprite MusicOn;
    public Sprite MusicOff;

    [Header("Helpers Settings")]
    public GameObject AndroidPanel;
    public GameObject PCPanel;

    private SoundManager _soundManager;
    private Animator _soundBtnAnimator;
    private Animator _musicBtnAnimator;
    private bool _musicSettingIsActive;
    private bool _soundSettingIsActive;
    private int _soundBtnOpenAnimHash;
    private int _soundBtnCloseAnimHash;
    private int _musicBtnOpenAnimHash;
    private int _musicBtnCloseAnimHash;

    public void Awake()
    {
        _soundManager = AllServices.Instance.GetService<SoundManager>();

        ActivateSliders();
        InitAnimatorsInfo();
        InitHelpers();
        //StartCoroutine(InitSoundAndMusic());
    }

    void Start()
    {
        GameManager.SwichGameState<MainMenuState>();
        StartMenu.SetActive(true);
        //WorldsChoose.SetActive (false);
        //LevelsChoose.SetActive (false);
        Shop.SetActive(false);
        HelperPanel.SetActive(false);
        InitSoundAndMusic();
    }

    public void OpenWorld(int world)
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        WorldsChoose.SetActive(false);
        LevelsChoose.SetActive(true);

        for (int i = 0; i < WorldLevel.Length; i++)
        {
            if (i == (world - 1))
                WorldLevel[i].SetActive(true);
            else
                WorldLevel[i].SetActive(false);
        }
    }

    public void OpenWorldChoose()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        StartMenu.SetActive(false);
        WorldsChoose.SetActive(true);
        LevelsChoose.SetActive(false);
    }

    public void OpenStartMenu()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        StartMenu.SetActive(true);
        //WorldsChoose.SetActive(false);
        Shop.SetActive(false);
    }

    public void OpenShop()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        StartMenu.SetActive(false);
        Shop.SetActive(true);
    }

    public void OpenHelpers()
    {
        SoundManager.PlaySfx(_soundManager.SoundClick);
        HelperPanel.SetActive(true);
    }

    public void OpenScene(string sceneName) => LoadSceneState.LoadScene(sceneName, true);

    public void MusicSettings()
    {
        if (_musicSettingIsActive)
            _musicBtnAnimator.Play(_musicBtnCloseAnimHash);
        else
            _musicBtnAnimator.Play(_musicBtnOpenAnimHash);
        _musicSettingIsActive ^= true;
    }

    public void SoundSettings()
    {
        if (_soundSettingIsActive)
            _soundBtnAnimator.Play(_soundBtnCloseAnimHash);
        else
            _soundBtnAnimator.Play(_soundBtnOpenAnimHash);
        _soundSettingIsActive ^= true;
    }

    public void SetMusicVolume(float val)
    {
        if (!_soundManager)
        {
            Debug.Log("_soundManager is null");
            return;
        }

        SaveInfoManager.SaveMusicVal(_soundManager.MusicVolume);
        _soundManager.MusicVolume = val;
        TurnMusic(val <= 0.0001f);
    }

    public void SetSoundVolume(float val)
    {
        if (!_soundManager)
        {
            Debug.Log("_soundManager is null");
            return;
        }

        SaveInfoManager.SaveSoundVal(_soundManager.SoundVolume);
        _soundManager.SoundVolume = val;
        TurnSound(val <= 0.0001f);
    }

    private void InitSoundAndMusic()
    {
        //yield return new WaitForSecondsRealtime(0.1f);

        float value = SaveInfoManager.LoadSoundVal();
        SoundSlider.value = value;
        SetSoundVolume(value);

        value = SaveInfoManager.LoadMusicVal();
        MusicSlider.value = value;
        SetMusicVolume(value);
    }

    private void InitHelpers()
    {
#if UNITY_STANDALONE
        PCPanel.SetActive(true);
        AndroidPanel.SetActive(false);
#else
        PCPanel.SetActive(false);
        AndroidPanel.SetActive(true);
#endif
    }

    private void TurnSound(bool condition) => Turn(condition, SoundImage, SoundOn, SoundOff);

    private void TurnMusic(bool condition) => Turn(condition, MusicImage, MusicOn, MusicOff);

    private void Turn(bool condition, Image img, Sprite on, Sprite off) => img.sprite = condition ? off : on;

    private void InitAnimatorsInfo()
    {
        _soundBtnAnimator = SoundSlider.GetComponent<Animator>();
        _musicBtnAnimator = MusicSlider.GetComponent<Animator>();

        _soundBtnOpenAnimHash = Animator.StringToHash("OpenSlider");
        _soundBtnCloseAnimHash = Animator.StringToHash("CloseSlider");
        _musicBtnOpenAnimHash = Animator.StringToHash("OpenSlider");
        _musicBtnCloseAnimHash = Animator.StringToHash("CloseSlider");
    }

    private void ActivateSliders()
    {
        SoundSlider.gameObject.SetActive(true);
        MusicSlider.gameObject.SetActive(true);
    }

}
