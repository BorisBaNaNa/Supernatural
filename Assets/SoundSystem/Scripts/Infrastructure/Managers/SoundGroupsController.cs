using SoundSystem.Scripts.ScriptableObjects;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem.Scripts.Infrastructure.Managers
{
    public class SoundGroupsController
    {
        [Serializable]
        public class SoundGroup
        {
            public SoundGroups GroupType => _groupType;
            public AudioSource Source => _globalSource;

            public float Volume
            {
                get => Val01FromMixer(_groupVolumeName);
                set => SetVal01ToMixer(_groupVolumeName, value);
            }

            [SerializeField] private SoundGroups _groupType;
            [SerializeField] private bool _isLoopingPlay;
            [SerializeField, Range(0.0001f, 1f)] private float _defaultVolume;

            private AudioMixer _mixer;
            private AudioSource _globalSource;
            private string _groupVolumeName;
            private string _groupPrefsName;

            public void Init(AudioSource globalSource, AudioMixer mixer)
            {
                _mixer = mixer;

                _groupVolumeName = $"{_groupType}Vol";
                _groupPrefsName = $"SoundSettings_{_groupType}";

                _globalSource = globalSource;
                _globalSource.loop = _isLoopingPlay;
            }

            public void Play(AudioClip clip, AudioSource targetSource = null)
            {
                if (clip == null)
                    return;
                if (targetSource == null)
                    targetSource = _globalSource;

                if (!_isLoopingPlay)
                    targetSource.PlayOneShot(clip);
                else
                {
                    targetSource.clip = clip;
                    targetSource.Play();
                }
            }

            public void SaveGroupInPrefs() => PlayerPrefs.SetFloat(_groupPrefsName, Volume);

            public void LoadGroupInPrefs() => Volume = PlayerPrefs.GetFloat(_groupPrefsName, _defaultVolume);


            private float Val01FromMixer(string mixerGroupName)
            {
                _mixer.GetFloat(mixerGroupName, out float mixerVol);
                return Mathf.Pow(10f, (mixerVol / _decibelMultiply));
            }

            private void SetVal01ToMixer(string mixerGroupName, float value)
            {
                float clampVal = Mathf.Clamp(value, 0.0001f, 1f);
                _mixer.SetFloat(mixerGroupName, _decibelMultiply * Mathf.Log10(clampVal));
            }
        }

        private readonly AudioMixer _mixer;
        private readonly GameObject _audioSourceContainer;
        private readonly SoundGroup[] _soundGroups;
        private const float _decibelMultiply = 20f;

        public SoundGroupsController(SoundSettings soundSettings, GameObject audioSourceContainer)
        {
            _mixer = soundSettings.Mixer;
            _soundGroups = soundSettings.SoundGroups;
            _audioSourceContainer = audioSourceContainer;

            foreach (SoundGroup soundGroup in _soundGroups)
                soundGroup.Init(CreateAudioSource(soundGroup.GroupType), _mixer);
        }

        public void Dispose()
        {
            foreach (SoundGroup soundGroup in _soundGroups)
                GameObject.Destroy(soundGroup.Source);
        }

        public SoundGroup GetGroup(SoundGroups groupType) => _soundGroups.FirstOrDefault(groupe => groupe.GroupType == groupType);

        public void SavePrefsSettings()
        {
            foreach (var soundGroup in _soundGroups)
                soundGroup.SaveGroupInPrefs();
        }

        /// <summary>
        /// Load settings from PlayerPrefs. Call only after Awake.
        /// </summary>
        public void LoadPrefsSettings()
        {
            foreach (var soundGroup in _soundGroups)
                soundGroup.LoadGroupInPrefs();
        }

        public AudioSource CreateAudioSource(SoundGroups group, GameObject sourceContainer = null)
        {
            if (sourceContainer == null)
                sourceContainer = _audioSourceContainer;

            AudioSource audioSource = sourceContainer.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(group.ToString()).First();
            audioSource.playOnAwake = false;
            return audioSource;
        }
    }
}
