using Cysharp.Threading.Tasks;
using SoundSystem.Scripts.ScriptableObjects;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem.Scripts.Infrastructure.Manages
{
    /// <summary>
    /// Контроллер для управления группами звуков.
    /// </summary>
    public class SoundGroupsController
    {
        private readonly AudioMixer _mixer;
        private readonly GameObject _audioSourceContainer;
        private readonly SoundGroup[] _soundGroups;
        private const float _decibelMultiply = 20f;

        /// <summary>
        /// Инициализация контроллера групп звуков.
        /// </summary>
        /// <param name="soundSettings">Настройки звука.</param>
        /// <param name="audioSourceContainer">Контейнер для источников звука.</param>
        public SoundGroupsController(SoundSettings soundSettings, GameObject audioSourceContainer)
        {
            _mixer = soundSettings.Mixer;
            _soundGroups = soundSettings.SoundGroups;
            _audioSourceContainer = audioSourceContainer;

            foreach (SoundGroup soundGroup in _soundGroups)
                soundGroup.Init(CreateAudioSource(soundGroup.GroupType), _mixer);
        }

        /// <summary>
        /// Освобождение ресурсов контроллера групп звуков.
        /// </summary>
        public void Dispose()
        {
            foreach (SoundGroup soundGroup in _soundGroups)
                soundGroup.Dispose();
        }

        /// <summary>
        /// Получение группы звуков по типу.
        /// </summary>
        /// <param name="groupType">Тип группы звуков.</param>
        /// <returns>Группа звуков.</returns>
        public SoundGroup GetGroup(SoundGroups groupType) => _soundGroups.FirstOrDefault(groupe => groupe.GroupType == groupType);

        /// <summary>
        /// Сохранение настроек всех групп звуков в PlayerPrefs.
        /// </summary>
        public void SavePrefsSettings()
        {
            foreach (var soundGroup in _soundGroups)
                soundGroup.SaveGroupInPrefs();
        }

        /// <summary>
        /// Загрузка настроек всех групп звуков из PlayerPrefs.
        /// </summary>
        public void LoadPrefsSettings()
        {
            foreach (var soundGroup in _soundGroups)
                soundGroup.LoadGroupInPrefs();
        }

        /// <summary>
        /// Создание источника звука.
        /// </summary>
        /// <param name="group">Группа звуков.</param>
        /// <param name="sourceContainer">Контейнер для источника звука.</param>
        /// <returns>Источник звука.</returns>
        public AudioSource CreateAudioSource(SoundGroups group, GameObject sourceContainer = null)
        {
            if (sourceContainer == null)
                sourceContainer = _audioSourceContainer;

            AudioSource audioSource = sourceContainer.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(group.ToString()).First();
            audioSource.playOnAwake = false;
            return audioSource;
        }

        /// <summary>
        /// Типы воспроизведения звука.
        /// </summary>
        public enum PlayType
        {
            /// <summary>
            /// Остановить текущий звук, затем воспроизвести новый.
            /// </summary>
            STOP_THEN_PLAY,
            /// <summary>
            /// Воспроизвести звук один раз.
            /// </summary>
            ONE_SHOT,
            /// <summary>
            /// Воспроизвести звук без прерывания текущего.
            /// </summary>
            NO_INTERRUPT
        }

        /// <summary>
        /// Группа звуков.
        /// </summary>
        [Serializable]
        public class SoundGroup
        {
            /// <summary>
            /// Событие, вызываемое при начале воспроизведения.
            /// </summary>
            public event Action OnStatedPlaying;

            /// <summary>
            /// Тип группы звуков.
            /// </summary>
            public SoundGroups GroupType => _groupType;

            /// <summary>
            /// Источник звука.
            /// </summary>
            public AudioSource Source => _globalSource;

            /// <summary>
            /// Флаг, указывающий, воспроизводится ли звук на основном источнике.
            /// </summary>
            public bool IsPlayingOnDefaultSource => _globalSource.isPlaying;

            /// <summary>
            /// Громкость звука.
            /// </summary>
            public float Volume
            {
                get => DecToVal01(_groupVolumeName);
                set => Val01ToDec(_groupVolumeName, value);
            }

            /// <summary>
            /// Тип группы звуков.
            /// </summary>
            [SerializeField] private SoundGroups _groupType;

            /// <summary>
            /// Громкость по умолчанию.
            /// </summary>
            [SerializeField] private float _defaultVolume;

            private AudioMixer _mixer;
            private AudioSource _globalSource;
            private string _groupVolumeName;
            private string _groupPrefsName;
            private SemaphoreSlim _playingSemaphore = new SemaphoreSlim(1, 1);

            /// <summary>
            /// Инициализация группы звуков.
            /// </summary>
            /// <param name="globalSource">Основной источник звука.</param>
            /// <param name="mixer">Микшер звука.</param>
            public void Init(AudioSource globalSource, AudioMixer mixer)
            {
                _mixer = mixer;
                _globalSource = globalSource;

                _groupVolumeName = $"{_groupType}Vol";
                _groupPrefsName = $"SoundSettings_{_groupType}";
            }

            /// <summary>
            /// Освобождение ресурсов группы звуков.
            /// </summary>
            public void Dispose()
            {
                GameObject.Destroy(_globalSource);
                _mixer = null;
            }

            /// <summary>
            /// Воспроизведение звука.
            /// </summary>
            /// <param name="clip">Звуковой клип.</param>
            /// <param name="loop">Флаг зацикливания.</param>
            /// <param name="playType">Тип воспроизведения.</param>
            /// <param name="targetSource">Целевой источник звука.</param>
            /// <param name="onStartedPlaying">Действие при начале воспроизведения.</param>
            /// <param name="onFinishedPlaying">Действие при завершении воспроизведения.</param>
            public void Play(AudioClip clip, bool loop = false, PlayType playType = PlayType.ONE_SHOT, AudioSource targetSource = null, Action onStartedPlaying = null, Action onFinishedPlaying = null)
            {
                if (clip == null)
                    return;
                if (targetSource == null)
                    targetSource = _globalSource;

                switch (playType)
                {
                    case PlayType.STOP_THEN_PLAY:
                        PlayOnTargetSource(clip, loop).Forget();
                        break;
                    case PlayType.NO_INTERRUPT:
                        if (targetSource.isPlaying)
                            return;
                        PlayOnTargetSource(clip, loop).Forget();
                        break;
                    case PlayType.ONE_SHOT:
                    default:
                        targetSource.PlayOneShot(clip);
                        break;
                }

                OnStatedPlaying?.Invoke();

                async UniTask PlayOnTargetSource(AudioClip clip, bool loop)
                {
                    targetSource.Stop();
                    targetSource.loop = loop;
                    targetSource.clip = clip;

                    await _playingSemaphore.WaitAsync();
                    targetSource.Play();
                    onStartedPlaying?.Invoke();
                    WaitEndPlaying(onFinishedPlaying, _playingSemaphore).Forget();
                }
            }

            /// <summary>
            /// Остановка воспроизведения звука.
            /// </summary>
            public void Stop() => _globalSource?.Stop();

            /// <summary>
            /// Сохранение настроек группы в PlayerPrefs.
            /// </summary>
            public void SaveGroupInPrefs() => PlayerPrefs.SetFloat(_groupPrefsName, Volume);

            /// <summary>
            /// Загрузка настроек группы из PlayerPrefs.
            /// </summary>
            public void LoadGroupInPrefs() => Volume = PlayerPrefs.GetFloat(_groupPrefsName, _defaultVolume);

            /// <summary>
            /// Ожидание завершения воспроизведения звука.
            /// </summary>
            /// <param name="onFinishedPlaying">Действие при завершении воспроизведения.</param>
            /// <param name="playingSemaphore">Семафор для управления доступом.</param>
            private async UniTask WaitEndPlaying(Action onFinishedPlaying, SemaphoreSlim playingSemaphore)
            {
                if (onFinishedPlaying == null)
                {
                    playingSemaphore.Release();
                    return;
                }

                await UniTask.WaitUntil(() => !_globalSource.isPlaying);
                onFinishedPlaying?.Invoke();
                playingSemaphore.Release();
            }

            /// <summary>
            /// Преобразование децибел в значение от 0 до 1.
            /// </summary>
            /// <param name="mixerGroupName">Имя группы микшера.</param>
            /// <returns>Значение громкости от 0 до 1.</returns>
            private float DecToVal01(string mixerGroupName)
            {
                _mixer.GetFloat(mixerGroupName, out float mixerVol);
                return Mathf.Pow(10f, (mixerVol / _decibelMultiply));
            }

            /// <summary>
            /// Преобразование значения от 0 до 1 в децибелы.
            /// </summary>
            /// <param name="mixerGroupName">Имя группы микшера.</param>
            /// <param name="value">Значение громкости от 0 до 1.</param>
            private void Val01ToDec(string mixerGroupName, float value)
            {
                float clampVal = Mathf.Clamp(value, 0.0001f, 1f);
                _mixer.SetFloat(mixerGroupName, _decibelMultiply * Mathf.Log10(clampVal));
            }
        }
    }
}
