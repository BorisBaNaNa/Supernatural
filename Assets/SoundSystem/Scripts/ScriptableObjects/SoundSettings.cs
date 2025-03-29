using SoundSystem.Scripts.Infrastructure.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewSoundSettings", menuName = "Sound/SettingsConfig")]
    public class SoundSettings : ScriptableObject
    {
        [field: SerializeField] public AudioMixer Mixer { get; private set; }
        [field: SerializeField] public SoundGroupsController.SoundGroup[] SoundGroups { get; private set; }
    }
}