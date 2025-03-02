using Assets.Supernatural.Scripts.Infrastructure;
using Assets.Supernatural.Scripts.Input;
using Assets.Supernatural.Scripts.Interfaces;
using UnityEngine;

public class Bootstraper : MonoBehaviour
{
    [SerializeField] private UIPlayerController _uiPlayerController;

    private void Awake()
    {
        InitializeInputController();
    }

    private void OnDestroy()
    {
        ServiceLocator.DisposeService<IPlayerInputController>();
    }

    private void InitializeInputController()
    {
#if UNITY_ANDROID
        ServiceLocator.RegService<IPlayerInputController>(_uiPlayerController);
#else
        _uiPlayerController.Disable();
        ServiceLocator.RegService<IPlayerInputController>(new KeyBoardPlayerController());
#endif
    }
}
