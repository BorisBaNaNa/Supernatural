using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    [CreateAssetMenu(fileName = "PlayerAttackAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerAttackAnimationsReferences")]
    public class PlayerAttackAnimationsReferences : ScriptableObject
    {
        public AnimationReferenceAsset MeleeAttackAnim { get; private set; }
        public AnimationReferenceAsset RangeAttackAnim { get; private set; }
    }
}
