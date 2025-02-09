using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public bool LoadOnClick = false;

    [SerializeField]
    private LoadBar ProgressBar;
    [SerializeField]
    private LoadBar RightWall;
    [SerializeField]
    private TextMeshProUGUI ProgressText;
    [SerializeField]
    private Button StartBtn;

    private AsyncOperation _asyncOperation;
    private string startprogressText;

    private void Awake()
    {
        Vector3 spawnPos = GameObject.FindWithTag("LevelStart").transform.position;
        Player player = AllServices.Instance.GetService<FactoryPlayer>().BuildPlayer(spawnPos);
        player.Inputs.Player.RangeAttack.Disable();
        player.Inputs.Player.MeleeAttack.Disable();
        LoadOnClick = GameManager.LoadOnClick;
    }

    private void Start()
    {
        startprogressText = ProgressText.text;
        StartCoroutine(AsyncSceneLoad(GameManager.LoadSceneName));
    }

    public IEnumerator AsyncSceneLoad(string sceneName)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        _asyncOperation.allowSceneActivation = !LoadOnClick;

        while (_asyncOperation.progress < 0.9f)
        {
            UpdateUI();
            yield return true;
        }

        UpdateUI();
        StartBtn.gameObject.SetActive(LoadOnClick);
    }

    private void UpdateUI()
    {
        float loadingProcess = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
        ProgressText.text = $"{startprogressText} {Mathf.FloorToInt(loadingProcess * 100)} %";
        ProgressBar.SetLoadState(loadingProcess);
        RightWall.SetLoadState(loadingProcess);
    }

    public void OpenLoadedScene()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}
