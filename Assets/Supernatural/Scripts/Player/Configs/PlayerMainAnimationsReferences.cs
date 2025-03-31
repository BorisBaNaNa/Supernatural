using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Configs
{
    [CreateAssetMenu(fileName = "PlayerMainAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerMainAnimationsReferences")]
    public class PlayerMainAnimationsReferences : ScriptableObject
    {
        [field: SerializeField] public AnimationReferenceAsset TakeDamage { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Death { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Ultimate { get; private set; }
    }
}
