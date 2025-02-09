using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
/*
* This is SoundManager
* In other script, you just need to call SoundManager.PlaySfx(AudioClip) to play the sound
*/
public class SoundManager : MonoBehaviour, IService
{
    public float SoundVolume
    {
        get
        {
            Mixer.GetFloat(SoundVolParametr, out var decibelsVal);
            return Mathf.Pow(10, decibelsVal / _valMultiplier);
        }

        set
        {
            float decibelsVal = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * _valMultiplier;
            Mixer.SetFloat(SoundVolParametr, decibelsVal);
        }
    }
    public float MusicVolume
    {
        get
        {
            Mixer.GetFloat(MusicVolParametr, out var decibelsVal);
            return Mathf.Pow(10, decibelsVal / _valMultiplier);
        }

        set
        {
            float decibelsVal = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * _valMultiplier;
            Mixer.SetFloat(MusicVolParametr, decibelsVal);
        }
    }

    [Header("Mixer settings")]
    public AudioMixer Mixer;
    public string MusicVolParametr = "MasterVol";
    public AudioSource MusicSource;
    public string SoundVolParametr = "MasterVol";
    public AudioSource SoundSource;

    [Header("Other settings")]
    [Tooltip("Play music clip when start")]
    public AudioClip MusicsMenu;

    [Tooltip("Place the sound in this to call it in another script by: SoundManager.PlaySfx(soundname);")]
    public AudioClip SoundClick;
    public AudioClip SoundGamefinish;
    public AudioClip SoundGameover;

    private float _valMultiplier = 20f;

    public void Initialize()
    {
        if (MusicSource == null || SoundSource == null)
            Debug.LogError("Sound is not initialized!");

        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySfx(AudioClip clip)
    {
        SoundManager instance = AllServices.Instance.GetService<SoundManager>();
        instance.PlaySound(clip, instance.SoundSource);
    }

    public static void PlayMusic(AudioClip clip)
    {
        SoundManager instance = AllServices.Instance.GetService<SoundManager>();
        instance.PlaySound(clip, instance.MusicSource);
    }

    private void PlaySound(AudioClip clip, AudioSource audioOut)
    {
        if (clip == null)
            return;

        if (audioOut == MusicSource)
        {
            audioOut.clip = clip;
            audioOut.Play();
        }
        else
            audioOut.PlayOneShot(clip);
    }
}
