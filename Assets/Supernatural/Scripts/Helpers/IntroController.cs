using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using static UnityEngine.InputSystem.InputAction;

public class IntroController : MonoBehaviour
{
    public VideoClip secondClip;
    public float _firstIntroDuration;
    public float _secondIntroDuration;

    private VideoPlayer _videoPlayer;
    private InputActions _inputActions;
    private AsyncOperation _loadOperation;
    private float _currentTime;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _inputActions = new InputActions();
        _inputActions.Intro.CloseIntro.performed += SkipIntro;
    }

    private void Start()
    {
        _loadOperation = SceneManager.LoadSceneAsync("MainMenu");
        _loadOperation.allowSceneActivation = false;

        StartCoroutine(StartDelegateWithDelay(_firstIntroDuration, () =>
        {
            _videoPlayer.clip = secondClip;
            _videoPlayer.Play();
            StartCoroutine(StartDelegateWithDelay(_secondIntroDuration, () => _loadOperation.allowSceneActivation = true));
        }));
    }

    private void OnEnable()
    {
        _inputActions.Intro.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Intro.Disable();
    }

    private IEnumerator StartDelegateWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private void SkipIntro(CallbackContext _)
    {
        if (_loadOperation.progress == 0.9f)
        {
            StopAllCoroutines();
            _videoPlayer.Stop();
            _loadOperation.allowSceneActivation = true;
        }
    }
}
