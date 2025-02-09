using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour
{
    private SoundManager _soundManager;

    void Start()
    {
        _soundManager = AllServices.Instance.GetService<SoundManager>();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SoundManager.PlaySfx(_soundManager.SoundClick);
        
        AllServices.Instance.GetService<GameManager>().SetupValues();
    }

    public void UnlockAll()
    {
        //PlayerPrefs.SetInt(GlobalValue.WorldReached, int.MaxValue);
        //for (int i = 1; i < 100; i++)
        //{
        //    PlayerPrefs.SetInt(i.ToString(), 10000);        //world i, unlock 10000 levels
        //}
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //SoundManager.PlaySfx(_soundManager.SoundClick);

        Debug.Log("Not implemented");
    }
}
