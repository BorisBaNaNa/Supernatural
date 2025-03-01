using UnityEngine;
using UnityEngine.Audio;
using static SoundSystem.Scripts.Infrastructure.Manages.SoundGroupsController;

namespace SoundSystem.Scripts.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject для хранения настроек звука.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoundSettings", menuName = "Sound/SettingsConfig")]
    public class SoundSettings : ScriptableObject
    {
        /// <summary>
        /// Микшер звука.
        /// </summary>
        [field: SerializeField] public AudioMixer Mixer { get; private set; }

        /// <summary>
        /// Массив групп звуков.
        /// </summary>
        [field: SerializeField] public SoundGroup[] SoundGroups { get; private set; }
    }
}
