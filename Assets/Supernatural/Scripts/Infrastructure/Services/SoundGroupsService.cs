using Assets.Supernatural.Scripts.Interfaces.Infrastructure;
using SoundSystem.Scripts.Infrastructure.Managers;
using SoundSystem.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Infrastructure.Services
{
    public class SoundGroupsService : IService
    {
        public SoundGroupsController SoundGroupsController { get; private set; }

        private GameObject _audioSourceContainer;

        public SoundGroupsService(SoundSettings soundSettings)
        {
            _audioSourceContainer = new("/ SoundSourceContainer");
            GameObject.DontDestroyOnLoad(_audioSourceContainer);

            SoundGroupsController = new(soundSettings, _audioSourceContainer);
        }

        public void Dispose()
        {
            SoundGroupsController.Dispose();
            SoundGroupsController = null;
            DestroySourceContainer();
        }

        private void DestroySourceContainer()
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(_audioSourceContainer);
#else
            Object.Destroy(_audioSourceContainer);
#endif
        }
    }
}