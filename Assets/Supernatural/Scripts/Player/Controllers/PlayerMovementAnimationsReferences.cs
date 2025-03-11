using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    [CreateAssetMenu(fileName = "PlayerMovementAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerMovementAnimationsReferences")]
    public class PlayerMovementAnimationsReferences : ScriptableObject
    {
        [field: SerializeField] public AnimationReferenceAsset Idle { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Walk { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Crouch { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Crouching { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset UnCrouch { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Jump { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Fall { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Land { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset SlideFaceToWall { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset SlideBackToWall { get; private set; }
    }
}
