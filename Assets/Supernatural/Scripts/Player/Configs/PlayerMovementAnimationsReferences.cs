using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Configs
{
    [CreateAssetMenu(fileName = "PlayerMovementAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerMovementAnimationsReferences")]
    public class PlayerMovementAnimationsReferences : ScriptableObject
    {
        [field: SerializeField] public AnimationReferenceAsset Idle { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Walk { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Jump { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Climb { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset ClimbIdle { get; private set; }
    }
}
