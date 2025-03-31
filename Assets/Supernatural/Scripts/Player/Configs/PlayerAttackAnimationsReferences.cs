using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Configs
{
    [CreateAssetMenu(fileName = "PlayerAttackAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerAttackAnimationsReferences")]
    public class PlayerAttackAnimationsReferences : ScriptableObject
    {
        [field: SerializeField] public AnimationReferenceAsset MeleeAttackAnim { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset RangeAttackAnim { get; private set; }
    }
}
