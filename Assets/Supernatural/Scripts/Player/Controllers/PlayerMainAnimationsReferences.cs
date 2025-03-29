using Spine.Unity;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    [CreateAssetMenu(fileName = "PlayerMainAnimationsReferences", menuName = "Supernatural/Configs/Animations/PlayerMainAnimationsReferences")]
    public class PlayerMainAnimationsReferences : ScriptableObject
    {
        public AnimationReferenceAsset TakeDamageAnim { get; private set; }
        public AnimationReferenceAsset Death { get; private set; }
        public AnimationReferenceAsset Win { get; private set; }
        public AnimationReferenceAsset Respawn { get; private set; }
    }
}
